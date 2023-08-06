using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;

namespace Auth.Infrastructure.TransferProtocol.TransferAbstractions;

public abstract class DeviceComparerAbstract : IDeviceComparer
{
    public abstract Task<List<string>> GetChangedProperties<T>(T oldObject, T newObject) where T : BaseDevice?;
    public abstract Task<bool> CompareDevices<T>(T oldObject, T newObject) where T : BaseDevice?;
}