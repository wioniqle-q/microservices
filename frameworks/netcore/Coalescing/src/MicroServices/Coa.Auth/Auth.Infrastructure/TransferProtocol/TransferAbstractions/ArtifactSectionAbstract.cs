using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;

namespace Auth.Infrastructure.TransferProtocol.TransferAbstractions;

public abstract class ArtifactSectionAbstract : IArtifactSection
{
    public abstract Task<string> GenerateRandomHexString(int length, CancellationToken cancellationToken = default);

    public abstract Task<List<string>> GetChangedProperties<T>(T oldObject, T newObject,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> RequiresUserConfirmation<T>(IEnumerable<T> changedProperties,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> CompareDevices<T>(T oldObject, T newObject,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> CheckDeviceInfo<T>(BaseUserEntitiy oldObject, BaseDevice newObject,
        CancellationToken cancellationToken = default);
}