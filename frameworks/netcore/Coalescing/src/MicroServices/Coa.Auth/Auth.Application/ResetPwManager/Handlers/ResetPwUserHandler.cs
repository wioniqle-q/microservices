using Auth.Application.ResetPwManager.Requests;
using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using MediatR;

namespace Auth.Application.ResetPwManager.Handlers;

public sealed class ResetPwUserHandler : IRequestHandler<ResetPwUserRequest, OutcomeValue>
{
    private readonly IResetAssesment _resetAssesment;

    public ResetPwUserHandler(
        IResetAssesment resetAssesment)
    {
        _resetAssesment = resetAssesment;
    }

    public async Task<OutcomeValue> Handle(ResetPwUserRequest request, CancellationToken cancellationToken)
    {
        var concealRequest = new BaseUserEntitiy
        {
            UserName = request.UserName,
            Password = request.OldPassword,
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

        var concealNewPassword = request.NewPassword;
        var resetAssesmentParams =
            await _resetAssesment.ResetAssesmentAsync(concealRequest, concealNewPassword, cancellationToken);

        return await Task.FromResult(new OutcomeValue
        {
            Outcome = resetAssesmentParams.Outcome
        });
    }
}