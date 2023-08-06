using Auth.Domain.Entities.MongoEntities;

namespace Auth.Infrastructure.TransferProtocol.TransferInterfaces;

public interface IDeviceComparer
{
    Task<List<string>> GetChangedProperties<T>(T oldObject, T newObject) where T : BaseDevice;
    Task<bool> CompareDevices<T>(T oldObject, T newObject) where T : BaseDevice;
}