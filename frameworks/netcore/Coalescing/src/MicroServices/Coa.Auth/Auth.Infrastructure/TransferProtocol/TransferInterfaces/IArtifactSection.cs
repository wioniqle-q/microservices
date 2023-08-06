using Auth.Domain.Entities.MongoEntities;

namespace Auth.Infrastructure.TransferProtocol.TransferInterfaces;

public interface IArtifactSection
{
    Task<string> GenerateRandomHexString(int length, CancellationToken cancellationToken = default);
    Task<List<string>> GetChangedProperties<T>(T oldObject, T newObject, CancellationToken cancellationToken = default);

    Task<bool> RequiresUserConfirmation<T>(IEnumerable<T> changedProperties,
        CancellationToken cancellationToken = default);

    Task<bool> CompareDevices<T>(T oldObject, T newObject, CancellationToken cancellationToken = default);

    Task<bool> CheckDeviceInfo<T>(BaseUserEntitiy oldObject, BaseDevice newObject,
        CancellationToken cancellationToken = default);
}