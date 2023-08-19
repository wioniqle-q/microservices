using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using Auth.Infrastructure.TransferProtocol.Transfers;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserTransaction.TransferVerification;

public class TransferAssesment : TransferAssesmentAbstract
{
    private readonly IConcealment _concealment;
    private readonly IEventBus _eventBus;
    private readonly Transfer _transfer;
    private readonly IUserHelper _userHelper;
    private readonly IUserSignature _userSignature;

    public TransferAssesment(
        IUserHelper userHelper,
        IUserSignature userSignature,
        IConcealment concealment,
        IEventBus eventBus,
        Transfer transfer)
    {
        _userHelper = userHelper;
        _userSignature = userSignature;
        _concealment = concealment;
        _eventBus = eventBus;
        _transfer = transfer;
    }

    public override async Task<OutcomeValue> AssessTransferAsync(BaseUserEntitiy user)
    {
        var assesConcealExists = await AssessTransferConcealment(user);
        if (string.IsNullOrWhiteSpace(assesConcealExists.UserProperty.RefreshToken) ||
            string.IsNullOrWhiteSpace(assesConcealExists.UserProperty.AccessToken))
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "TransferAssesment failed to be cred parsed, cloaking failed"
                });

        Console.WriteLine("TransferAssesment cloaking passed" + assesConcealExists.UserProperty.RefreshToken);

        var getUserId = await _userSignature.GetTokenNameIdentifier(assesConcealExists.UserProperty.RefreshToken);
        Console.WriteLine("TransferAssesment g cloaking passed" + getUserId);

        var convertGuid = ConvertStringToGuid(getUserId);
        if (convertGuid == Guid.Empty)
            return await new ValueTask<OutcomeValue>(
                new OutcomeValue
                {
                    Outcome = "TransferAssesment failed to be parsed, g cloaking failed"
                });

        var assesExists = await _userHelper.GetUserByIdAsync(convertGuid, CancellationToken.None);
        if (assesExists is null || assesExists.UserProperty.IsDeleted)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                Outcome = "Transfer user not present"
            });

        var transferBaseUserEntity = new BaseUserEntitiy
        {
            UserId = assesExists.UserId
        };
        
        assesConcealExists.Device = new BaseDevice
        {
            DeviceId = "1",
            DeviceType = "1",
            DeviceLocation = "1",
            DeviceLocationTimeZone = "1",
            DeviceNetworkName = "1",
            DeviceNetworkIp = "1",
            DeviceNetworkMac = "1",
            DeviceHardwareId = "1",
            DeviceHardwareMotherboardName = "1",
            DeviceHardwareOsName = "1",
            DeviceProcessorName = "1",
            DeviceGraphicsName = "1"
        };
        
        var transferValidation = await _transfer.ValidateTransferRefreshToken(
            assesConcealExists.UserProperty.RefreshToken, transferBaseUserEntity, assesConcealExists.Device,
            CancellationToken.None);

        if (transferValidation.Status is true)
            return await new ValueTask<OutcomeValue>(new OutcomeValue
            {
                UniqueResId = assesExists.UserId.ToString(),
                Outcome = "TransferAssesment passed",
                RefreshToken = transferValidation.Description
            });

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

    protected virtual async ValueTask<BaseUserEntitiy> AssessTransferConcealment(BaseUserEntitiy user)
    {
        if (string.IsNullOrWhiteSpace(user.UserProperty.RefreshToken) ||
            string.IsNullOrWhiteSpace(user.UserProperty.AccessToken))
            return new BaseUserEntitiy();

        var baseUser = new BaseUserEntitiy
        {
            UserProperty = new BaseUserProperty
            {
                RefreshToken = await _concealment.RevealAsync(user.UserProperty.RefreshToken, null, null),
                AccessToken = await _concealment.RevealAsync(user.UserProperty.AccessToken, null, null)
            }
        };

        return baseUser;
    }

    private static Guid ConvertStringToGuid(string guidString)
    {
        if (string.IsNullOrEmpty(guidString))
            return Guid.Empty;

        return !Guid.TryParse(guidString, out var result) ? Guid.Empty : result;
    }
}