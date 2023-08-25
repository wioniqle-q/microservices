using System.Security.Cryptography;
using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using Auth.Infrastructure.PasswordObfuscation;
using Auth.Infrastructure.TransferProtocol.Transfers;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserTransaction.UserVerification;

public class CredentialAssesment : CredentialAssesmentAbstract
{
    private readonly IConcealment _concealment;
    private readonly IEventBus _eventBus;
    private readonly Transfer _transfer;
    private readonly IUserHelper _userHelper;
    private readonly IUserSignature _userSignature;

    public CredentialAssesment(
        IConcealment concealment,
        IUserHelper userHelper, 
        IUserSignature userSignature,
        IEventBus eventBus,
        Transfer transfer)
    {
        _concealment = concealment;
        _userHelper = userHelper;
        _userSignature = userSignature;
        _eventBus = eventBus;
        _transfer = transfer;
    }

    public override async Task<OutcomeValue> AssessAsync(BaseUserEntitiy user)
    {
        var assesParams = await AssessPassword(user);
        if (string.IsNullOrEmpty(assesParams.Outcome))
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "Credential failed to be parsed, cloaking failed"
                });

        return await new ValueTask<OutcomeValue>(assesParams);
    }

    protected virtual async Task<OutcomeValue> AssessPassword(BaseUserEntitiy user)
    {
        var assesConcealExists = await AssessConcealment(user);
        if (string.IsNullOrWhiteSpace(assesConcealExists.UserName) ||
            string.IsNullOrWhiteSpace(assesConcealExists.Password))
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "Credential failed to be parsed, cloaking failed"
                });

        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, assesConcealExists.UserName);

        var assesExists = await _userHelper.FindUserByQueryAsync(filter, CancellationToken.None);
        if (assesExists is null || assesExists.UserProperty.IsDeleted)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome = "User not present"
            });

        var transferBaseUserEntity = new BaseUserEntitiy
        {
            UserId = assesExists.UserId
        };

        var transferBaseDevice = new BaseDevice
        {
            UserId = assesExists.UserId,
            DeviceId = assesConcealExists.Device.DeviceId,
            DeviceType = assesConcealExists.Device.DeviceType,
            DeviceLocation = assesConcealExists.Device.DeviceLocation,
            DeviceLocationTimeZone = assesConcealExists.Device.DeviceLocationTimeZone,
            DeviceNetworkName = assesConcealExists.Device.DeviceNetworkName,
            DeviceNetworkIp = assesConcealExists.Device.DeviceNetworkIp,
            DeviceNetworkMac = assesConcealExists.Device.DeviceNetworkMac,
            DeviceHardwareId = assesConcealExists.Device.DeviceHardwareId,
            DeviceHardwareMotherboardName = assesConcealExists.Device.DeviceHardwareMotherboardName,
            DeviceHardwareOsName = assesConcealExists.Device.DeviceHardwareOsName,
            DeviceProcessorName = assesConcealExists.Device.DeviceProcessorName,
            DeviceGraphicsName = assesConcealExists.Device.DeviceGraphicsName
        };

        var transferValidation = await _transfer.ValidateTransferRefreshToken(
            assesConcealExists.UserProperty.RefreshToken, transferBaseUserEntity, transferBaseDevice,
            CancellationToken.None);

        if (transferValidation.Status is not true)
        {
            var faAsync = await UpdateUser2FaAsync(assesExists, CancellationToken.None);
            if (faAsync is not true)
                return await new ValueTask<OutcomeValue>(new OutcomeValue
                {
                    Outcome =
                        "An unidentified situation has occurred on system, Please contact the system administrator."
                });

            var unAuthorisedEvent = new UnAuthorisedEvent
            {
                UserName = assesExists.UserName,
                FirstName = assesExists.FirstName,
                MiddleName = assesExists.MiddleName,
                LastName = assesExists.LastName,
                Email = assesExists.Email,
                UserProperty = assesExists.UserProperty,
                Device = assesConcealExists.Device
            };

            await _eventBus.PublishAsync(unAuthorisedEvent, default);

            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome =
                    "Wew have detected unusual activity on your account. Please confirm the verification link sent to your email address to complete the verification step of our system. If you are aware of a problem with your account, please do not forget to contact us. Our system will take action to protect you. Thank you for your patience.",
                Description = transferValidation.Description
            });
        }

        switch (assesExists.UserProperty)
        {
            case { IsLocked: true, IsEmailConfirmed: false }:
            {
                var unAuthorisedEvent = new UnAuthorisedEvent
                {
                    UserName = assesExists.UserName,
                    FirstName = assesExists.FirstName,
                    MiddleName = assesExists.MiddleName,
                    LastName = assesExists.LastName,
                    Email = assesExists.Email,
                    UserProperty = assesExists.UserProperty,
                    Device = assesConcealExists.Device
                };

                await _eventBus.PublishAsync(unAuthorisedEvent, default);

                return await new ValueTask<OutcomeValue>(
                    new OutcomeValue
                    {
                        Outcome =
                            "Email confirmation is required to activate your account. Please double check your email address."
                    });
            }
            case { IsLocked: true, Require2Fa: true }:
                return await new ValueTask<OutcomeValue>(
                    new OutcomeValue
                    {
                        Outcome =
                            "Please verify the link in your email. You are now in the 2-step verification process. If you prefer to run a 2-step verification every 30 or 90 days, contact the board administrator. The default setting is 1 day."
                    });
        }

        var verifySignature = await _userSignature.ValidateUserTokenLifeTime(assesExists.UserProperty.Token);
        if (verifySignature is false)
        {
            var currenTime = DateTime.UtcNow;
            var updatedTime = currenTime.AddDays(14);
            var randomTransactionString = await GenerateRandomHexString();

            var userSignatureInfo = new BaseUserSignatureEntitiy
            {
                TransactionId = randomTransactionString,
                TrialStatus = true,
                IsAuthorized = false,
                CustomAuthorization = "TRIAL-USER",
                IsBlocked = false,
                OccurrenceTime = currenTime.ToString("O"),
                EnrollmentDate = updatedTime.ToString("O"),
                TrialDate = updatedTime
            };

            var reSignProcess = await _userHelper
                .UpdateUserTokenAsync(new BaseUserEntitiy { UserName = assesExists.UserName }, userSignatureInfo,
                    CancellationToken.None);
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome = reSignProcess
            });
        }

        var attackSignature = await _userHelper
            .ValidateUserLastLoginAsync(new BaseUserEntitiy
                {
                    UserName = assesExists.UserName, UserProperty = new BaseUserProperty
                        { TimeZone = assesConcealExists.UserProperty.TimeZone }
                }, DateTime.UtcNow,
                CancellationToken.None);
        if (attackSignature is false)
        {
            var faAsync = await UpdateUser2FaAsync(assesExists, CancellationToken.None);
            if (faAsync is not true)
                return await new ValueTask<OutcomeValue>(new OutcomeValue
                {
                    Outcome =
                        "An unidentified situation has occurred on production, Please contact the system administrator."
                });

            var unAuthorisedEvent = new UnAuthorisedEvent
            {
                UserName = assesExists.UserName,
                FirstName = assesExists.FirstName,
                MiddleName = assesExists.MiddleName,
                LastName = assesExists.LastName,
                Email = assesExists.Email,
                UserProperty = assesExists.UserProperty,
                Device = assesConcealExists.Device
            };

            await _eventBus.PublishAsync(unAuthorisedEvent, default);

            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome =
                    "An unidentified situation has occurred on your account, please verify the link in the e-mail address you received to unlock your account."
            });
        }

        var assesObfuscation = await AssesObfuscation(assesConcealExists.Password,
            new BaseUserEntitiy
            {
                UserName = assesExists.UserName, Password = assesExists.Password,
                UserProperty = new BaseUserProperty
                {
                    TimeZone = assesConcealExists.UserProperty.TimeZone,
                    Require2Fa = assesExists.UserProperty.Require2Fa
                }
            });
        return await new ValueTask<OutcomeValue>(new OutcomeValue
        {
            UniqueResId = assesExists.UserId.ToString(),
            Outcome = assesObfuscation.Outcome!,
            RefreshToken = transferValidation.Description
        });
    }

    protected virtual async ValueTask<OutcomeValue> AssesObfuscation(string? password, BaseUserEntitiy user)
    {
        if (password is null || string.IsNullOrWhiteSpace(user.Password))
            return new OutcomeValue
            {
                Outcome = "Your password cannot be a blank, please try again"
            };

        var revealHash = await _concealment.RevealAsync(user.Password, null, null);

        var verifyPassword = await ObfuscatePassword.Verify(revealHash, password);
        if (verifyPassword is not true)
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "You have entered an incorrect password, please try again"
                });

        var loginUpdate = await _userHelper
            .SaveUserLastLoginAsync(
                new BaseUserEntitiy
                {
                    UserName = user.UserName,
                    UserProperty = new BaseUserProperty
                        { TimeZone = user.UserProperty.TimeZone, Require2Fa = user.UserProperty.Require2Fa }
                }, CancellationToken.None);
        if (loginUpdate is not true)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome =
                    "An unidentified situation has occurred on assesment, Please contact the system administrator."
            });

        return await new ValueTask<OutcomeValue>(new OutcomeValue
        {
            Outcome = "User has successfully logged on"
        });
    }

    protected virtual async Task<bool> UpdateUser2FaAsync(BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default)
    {
        var filter2Fa = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, baseUserEntitiy.UserName);
        var update2Fa = Builders<BaseUserEntitiy>.Update
            .Set(x => x.UserProperty.IsLocked, true)
            .Set(x => x.UserProperty.Require2Fa, true)
            .Set(x => x.UserProperty.IsEmailConfirmed, false);

        var result = await _userHelper.UpdateUserAsync(filter2Fa, update2Fa, null!, CancellationToken.None);
        return result;
    }

    protected virtual async ValueTask<BaseUserEntitiy> AssessConcealment(BaseUserEntitiy user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            return new BaseUserEntitiy();

        var baseUser = new BaseUserEntitiy
        {
            UserName = await _concealment.RevealAsync(user.UserName, null, null),
            Password = await _concealment.RevealAsync(user.Password, null, null),
            UserProperty = new BaseUserProperty
            {
                TimeZone = await _concealment.RevealAsync(user.UserProperty.TimeZone, null, null),
                AccessToken = await _concealment.RevealAsync(user.UserProperty.AccessToken, null, null),
                RefreshToken = await _concealment.RevealAsync(user.UserProperty.RefreshToken, null, null)
            },
            Device = new BaseDevice
            {
                DeviceId = await _concealment.RevealAsync(user.Device.DeviceId, null, null),
                DeviceType = await _concealment.RevealAsync(user.Device.DeviceType, null, null),
                DeviceLocation = await _concealment.RevealAsync(user.Device.DeviceLocation, null, null),
                DeviceLocationTimeZone = await _concealment.RevealAsync(user.Device.DeviceLocationTimeZone, null, null),
                DeviceNetworkName = await _concealment.RevealAsync(user.Device.DeviceNetworkName, null, null),
                DeviceNetworkIp = await _concealment.RevealAsync(user.Device.DeviceNetworkIp, null, null),
                DeviceNetworkMac = await _concealment.RevealAsync(user.Device.DeviceNetworkMac, null, null),
                DeviceHardwareId = await _concealment.RevealAsync(user.Device.DeviceHardwareId, null, null),
                DeviceHardwareMotherboardName = await _concealment
                    .RevealAsync(user.Device.DeviceHardwareMotherboardName, null, null),
                DeviceHardwareOsName = await _concealment.RevealAsync(user.Device.DeviceHardwareOsName, null, null),
                DeviceProcessorName = await _concealment.RevealAsync(user.Device.DeviceProcessorName, null, null),
                DeviceGraphicsName = await _concealment.RevealAsync(user.Device.DeviceGraphicsName, null, null)
            }
        };

        return baseUser;
    }

    protected virtual async ValueTask<string> GenerateRandomHexString(int length = 36)
    {
        var randomBytes = new byte[length / 2];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return await new ValueTask<string>(BitConverter.ToString(randomBytes).Replace("-", string.Empty).ToLower());
    }
}