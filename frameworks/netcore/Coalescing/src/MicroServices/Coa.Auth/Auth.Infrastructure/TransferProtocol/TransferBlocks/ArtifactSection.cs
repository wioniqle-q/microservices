using System.Security.Cryptography;
using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferAbstractions;
using Auth.Infrastructure.TransferProtocol.TransferUtilitiy;

namespace Auth.Infrastructure.TransferProtocol.TransferBlocks;

public sealed class ArtifactSection : ArtifactSectionAbstract
{
    private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();
    private static readonly DeviceComparer<BaseDevice> DeviceComparer = new();

    private static readonly List<string> ConfirmationProperties = new()
    {
        "DeviceId",
        "DeviceType",
        "DeviceLocation",
        "DeviceNetworkMac",
        "DeviceHardwareId",
        "DeviceHardwareMotherboardName",
        "DeviceHardwareOsName",
        "DeviceGraphicsName"
    };

    public override async Task<string> GenerateRandomHexString(int length,
        CancellationToken cancellationToken = default)
    {
        var randomBytes = new byte[length / 2];
        RandomNumberGenerator.GetBytes(randomBytes);
        return await Task.FromResult(BitConverter.ToString(randomBytes).Replace("-", string.Empty));
    }

    public override async Task<List<string>> GetChangedProperties<T>(T oldObject, T newObject,
        CancellationToken cancellationToken = default)
    {
        return await DeviceComparer.GetChangedProperties(oldObject as BaseDevice, newObject as BaseDevice);
    }

    public override async Task<bool> RequiresUserConfirmation<T>(IEnumerable<T> changedProperties,
        CancellationToken cancellationToken = default)
    {
        var stringChangedProperties = changedProperties
            .Select(p => p?.ToString())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        if (stringChangedProperties.Count == 0) return await Task.FromResult(true);

        var requiresConfirmation = stringChangedProperties.Except(ConfirmationProperties).Any();
        return await Task.FromResult(requiresConfirmation);
    }

    public override async Task<bool> CompareDevices<T>(T oldObject, T newObject,
        CancellationToken cancellationToken = default)
    {
        return await DeviceComparer.CompareDevices(oldObject as BaseDevice, newObject as BaseDevice);
    }

    public override async Task<bool> CheckDeviceInfo<T>(BaseUserEntitiy oldObject, BaseDevice newObject,
        CancellationToken cancellationToken = default)
    {
        if (oldObject.Device.DeviceId != newObject.DeviceId)
            return false;

        var compareDevices = await CompareDevices(oldObject.Device, newObject, cancellationToken);
        if (compareDevices is not false) return true;

        var changedProperties =
            await GetChangedProperties(oldObject.Device, newObject, cancellationToken);
        var requiresConfirmation =
            await RequiresUserConfirmation(changedProperties, cancellationToken);

        return requiresConfirmation;
    }
}