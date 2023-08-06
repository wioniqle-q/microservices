using Auth.Application.InsertManager.Requests;
using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using MediatR;

namespace Auth.Application.InsertManager.Handlers;

public sealed class InsertUserHandler : IRequestHandler<InsertUserRequest, OutcomeValue>
{
    private readonly IInsertAssesment _insertAssesment;

    public InsertUserHandler(
        IInsertAssesment insertAssesment)
    {
        _insertAssesment = insertAssesment;
    }

    public async Task<OutcomeValue> Handle(InsertUserRequest request, CancellationToken cancellationToken)
    {
        var concealRequest = new BaseUserEntitiy
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            UserProperty = new BaseUserProperty
            {
                TimeZone = request.TimeZone
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

        var insertAssessmentParams = await _insertAssesment.InsertAssesmentAsync(concealRequest, cancellationToken);

        return await Task.FromResult(new OutcomeValue
        {
            Outcome = insertAssessmentParams.Outcome
        });
    }
}