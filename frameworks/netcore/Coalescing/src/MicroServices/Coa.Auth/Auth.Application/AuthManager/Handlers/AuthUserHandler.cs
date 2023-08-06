using Auth.Application.AuthManager.Requests;
using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using MediatR;

namespace Auth.Application.AuthManager.Handlers;

public sealed class AuthUserHandler : IRequestHandler<AuthUserRequest, OutcomeValue>
{
    private readonly ICredentialAssesment _credentialAssesment;

    public AuthUserHandler(
        ICredentialAssesment credentialAssesment
    )
    {
        _credentialAssesment = credentialAssesment;
    }

    public async Task<OutcomeValue> Handle(AuthUserRequest request, CancellationToken cancellationToken)
    {
        var concealRequest = new BaseUserEntitiy
        {
            UserName = request.UserName,
            Password = request.Password,
            UserProperty = new BaseUserProperty
            {
                TimeZone = request.UserTimeZone,
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken
            },
            Device = new BaseDevice
            {
                DeviceId = request.Device.DeviceId,
                DeviceType = request.Device.DeviceType,
                DeviceLocation = request.Device.DeviceLocation,
                DeviceLocationTimeZone =
                    request.Device.DeviceLocationTimeZone,
                DeviceNetworkName = request.Device.DeviceNetworkName,
                DeviceNetworkIp = request.Device.DeviceNetworkIp,
                DeviceNetworkMac = request.Device.DeviceNetworkMac,
                DeviceHardwareId = request.Device.DeviceHardwareId,
                DeviceHardwareMotherboardName =
                    request.Device.DeviceHardwareMotherboardName,
                DeviceHardwareOsName =
                    request.Device.DeviceHardwareOsName,
                DeviceProcessorName =
                    request.Device.DeviceProcessorName,
                DeviceGraphicsName = request.Device.DeviceGraphicsName
            }
        };

        var assesParams = await _credentialAssesment.AssessAsync(concealRequest).ConfigureAwait(false);

        return await Task.FromResult(new OutcomeValue
        {
            Outcome = assesParams.Outcome,
            UniqueResId = assesParams.UniqueResId,
            Description = assesParams.Description,
            SubTopic = assesParams.SubTopic,
            RsaPublicKey = assesParams.RsaPublicKey,
            AccessToken = assesParams.AccessToken,
            RefreshToken = assesParams.RefreshToken,
            ClientAccessToken = assesParams.ClientAccessToken,
            ExceptionId = assesParams.ExceptionId,
            ExceptionType = assesParams.ExceptionType
        });
    }
}