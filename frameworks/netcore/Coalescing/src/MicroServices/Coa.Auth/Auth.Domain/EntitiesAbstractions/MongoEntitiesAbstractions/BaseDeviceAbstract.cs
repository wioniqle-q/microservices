using Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;

public abstract class BaseDeviceAbstract : IDevices
{
    // User Devices (User)
    public virtual Guid UserId { get; set; }
    public virtual string DeviceId { get; set; } = null!;
    public virtual string DeviceType { get; set; } = null!;

    // Device Location (GPS)
    public virtual string DeviceLocation { get; set; } = null!;
    public virtual string DeviceLocationTimeZone { get; set; } = null!;

    // Device Network (Internet)
    public virtual string DeviceNetworkName { get; set; } = null!;
    public virtual string DeviceNetworkIp { get; set; } = null!;
    public virtual string DeviceNetworkMac { get; set; } = null!;

    // Device Hardware (System)
    public virtual string DeviceHardwareId { get; set; } = null!;
    public virtual string DeviceHardwareMotherboardName { get; set; } = null!;
    public virtual string DeviceHardwareOsName { get; set; } = null!;

    // Device Processor (CPU)
    public virtual string DeviceProcessorName { get; set; } = null!;

    // Device Graphics (GPU)
    public virtual string DeviceGraphicsName { get; set; } = null!;
}