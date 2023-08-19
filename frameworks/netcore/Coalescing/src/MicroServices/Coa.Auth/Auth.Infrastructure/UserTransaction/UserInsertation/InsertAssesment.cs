using System.Security.Claims;
using System.Security.Cryptography;
using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.AdlemanProtocol.AdlemanInterfaces;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.GuidProtocol.GuidInterfaces;
using Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.EndpointOptions;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserTransaction.UserInsertation;

public class InsertAssesment : InsertAssesmentAbstract
{
    private readonly IAdlemanIdentity _adleman;
    private readonly IConcealment _concealment;
    private readonly IEndpointCredential _endpointCredentials;
    private readonly EndpointOption _endpointOption;
    private readonly IEventBus _eventBus;
    private readonly IGuid _guid;
    private readonly IUserHelper _userHelper;
    private readonly IUserSignature _userSignature;

    public InsertAssesment(
        IEndpointCredential endpointCredentials,
        IGuid guid,
        IConcealment concealment,
        IUserSignature userSignature,
        IAdlemanIdentity adleman,
        IUserHelper userHelper,
        IEventBus eventBus,
        EndpointOption endpointOption
    )
    {
        _endpointCredentials = endpointCredentials;
        _guid = guid;
        _concealment = concealment;
        _userSignature = userSignature;
        _adleman = adleman;
        _userHelper = userHelper;
        _eventBus = eventBus;
        _endpointOption = endpointOption;
    }

    public override async Task<OutcomeValue> InsertAssesmentAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var assesParams = await AssesInsert(user, cancellationToken);
        if (assesParams.Outcome is null)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome = "Unable to parse credential"
            });

        return await new ValueTask<OutcomeValue>(assesParams);
    }

    protected virtual async Task<OutcomeValue> AssesInsert(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var assesConcealExists = await AssessConcealment(user);
        if (string.IsNullOrWhiteSpace(assesConcealExists.UserName) ||
            string.IsNullOrWhiteSpace(assesConcealExists.FirstName) ||
            string.IsNullOrWhiteSpace(assesConcealExists.MiddleName) ||
            string.IsNullOrWhiteSpace(assesConcealExists.LastName) ||
            string.IsNullOrWhiteSpace(assesConcealExists.Email) ||
            string.IsNullOrWhiteSpace(assesConcealExists.Password))
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "Credential cannot be parsed, concealment failed"
                });

        var assesUserExists = await AssessUserExists(assesConcealExists);
        if (assesUserExists is not false)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome = "Existing user"
            });

        var userId = await _guid.GenerateGuidAsync();
        var currentTime = DateTime.UtcNow;
        var endpointTime = currentTime.AddMinutes(10);
        var randomTransactionString = await GenerateRandomHexString();
        var randomDeviceId = await GenerateRandomHexString(6);

        var keyPair = await _adleman.CreateKeyPairAsync(2048);
        var encryptRsa = await _adleman.EncryptAsync(assesConcealExists.LastName, keyPair.publicKey);

        var userRsa = new BaseUserRsa
        {
            RsaPublicKey = await _concealment.ConcealAsync(keyPair.publicKey, null, null),
            RsaPrivateKey = await _concealment.ConcealAsync(keyPair.privateKey, null, null),
            RsaValidateKey = await _concealment.ConcealAsync(encryptRsa, null, null)
        };

        var userDevice = new BaseDevice
        {
            UserId = userId,
            DeviceId = randomDeviceId,
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

        var userSignatureInfo = new BaseUserSignatureEntitiy
        {
            TransactionId = randomTransactionString,
            TrialStatus = true,
            IsAuthorized = false,
            CustomAuthorization = "TRIAL-USER",
            IsBlocked = false,
            OccurrenceTime = currentTime.ToString("O"),
            EnrollmentDate = currentTime.AddMinutes(10).ToString("O"),
            TrialDate = currentTime.AddDays(14)
        };

        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(_endpointOption.TransactionId, _endpointOption.TransactionId),
            new Claim(_endpointOption.ClientName, _endpointOption.ClientName),
            new Claim(_endpointOption.PermissionType, _endpointOption.PermissionType),
            new Claim(_endpointOption.PermissionName, _endpointOption.PermissionName),
            new Claim(_endpointOption.PermissionValue, _endpointOption.PermissionValue),
            new Claim(_endpointOption.SpecialValue, _endpointOption.SpecialValue),
            new Claim(_endpointOption.CreatedDate, currentTime.ToString("O")),
            new Claim(_endpointOption.ExpirationDate, endpointTime.ToString("O"))
        });

        var refreshToken = await _userSignature
                .GenerateUserRefreshToken(string.Concat(userId), currentTime.AddMinutes(10), userSignatureInfo);
        // access'll add

        var endpointToken = await _endpointCredentials
            .GenerateEndpointToken(endpointTime, claimsIdentity);

        var userProperties = new BaseUserProperty
        {
            UserId = userId,
            IsEmailConfirmed = false,
            IsLocked = true,
            IsDeleted = false,
            Token = string.Empty,
            CreatedAt = currentTime,
            LastLogin = string.Empty,
            TimeZone = assesConcealExists.UserProperty.TimeZone,
            LoginTimeSpan = 1,
            Require2Fa = true,
            DeviceId = randomDeviceId,
            AccessToken = string.Empty,
            RefreshToken = refreshToken,
            RefreshTokens = new List<string>()
        };

        var userEntity = new BaseUserEntitiy
        {
            UserId = userId,
            UserName = assesConcealExists.UserName,
            FirstName = assesConcealExists.FirstName,
            MiddleName = assesConcealExists.MiddleName,
            LastName = assesConcealExists.LastName,
            Email = assesConcealExists.Email,
            Password = assesConcealExists.Password,
            UserProperty = userProperties,
            Device = userDevice,
            UserRsa = userRsa
        };

        var insertedUser = await _userHelper.CreateUserAsync(userEntity, userSignatureInfo, cancellationToken);

        var assessUser = new InsertedEvent
        {
            UserName = userEntity.UserName,
            FirstName = userEntity.FirstName,
            MiddleName = userEntity.MiddleName,
            LastName = userEntity.LastName,
            Email = userEntity.Email,
            UserProperty = userProperties
        };

        await _eventBus.PublishAsync(assessUser, cancellationToken);

        return await new ValueTask<OutcomeValue>(new OutcomeValue
        {
            UniqueResId = string.Concat(userId),
            DeviceId = randomDeviceId,
            ClientAccessToken = endpointToken.Token,
            RsaPublicKey = insertedUser.RsaPublicKey,
            RefreshToken = insertedUser.RefreshToken,
            Outcome = "User created successfully"
        });
    }

    protected virtual async ValueTask<BaseUserEntitiy> AssessConcealment(BaseUserEntitiy user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.FirstName) ||
            string.IsNullOrWhiteSpace(user.MiddleName) || string.IsNullOrWhiteSpace(user.LastName) ||
            string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            return await new ValueTask<BaseUserEntitiy>(new BaseUserEntitiy());

        Console.WriteLine("AssessConcealment" + user.UserName);

        return new BaseUserEntitiy
        {
            UserName = await _concealment.RevealAsync(user.UserName, null, null),
            FirstName = await _concealment.RevealAsync(user.FirstName, null, null),
            MiddleName = await _concealment.RevealAsync(user.MiddleName, null, null),
            LastName = await _concealment.RevealAsync(user.LastName, null, null),
            Email = await _concealment.RevealAsync(user.Email, null, null),
            Password = await _concealment.RevealAsync(user.Password, null, null),
            UserProperty = new BaseUserProperty
            {
                TimeZone = await _concealment.RevealAsync(user.UserProperty.TimeZone, null, null)
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
                DeviceHardwareMotherboardName =
                    await _concealment.RevealAsync(user.Device.DeviceHardwareMotherboardName, null, null),
                DeviceHardwareOsName = await _concealment.RevealAsync(user.Device.DeviceHardwareOsName, null, null),
                DeviceProcessorName = await _concealment.RevealAsync(user.Device.DeviceProcessorName, null, null),
                DeviceGraphicsName = await _concealment.RevealAsync(user.Device.DeviceGraphicsName, null, null)
            }
        };
    }

    protected virtual async ValueTask<bool> AssessUserExists(BaseUserEntitiy user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            return await new ValueTask<bool>(false);

        var filter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, user.UserName);

        var userExists = await _userHelper.FindUserByQueryAsync(filter);
        if (userExists is null)
            return await new ValueTask<bool>(false);

        return await new ValueTask<bool>(true);
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