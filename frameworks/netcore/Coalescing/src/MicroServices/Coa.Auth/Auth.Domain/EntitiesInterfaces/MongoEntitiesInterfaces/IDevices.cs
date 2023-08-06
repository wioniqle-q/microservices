namespace Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

public interface IDevices
{
    // User Devices (User)
    public Guid UserId { get; set; }
    public string DeviceId { get; set; }
    public string DeviceType { get; set; }

    // Device Location (GPS)
    public string DeviceLocation { get; set; }
    public string DeviceLocationTimeZone { get; set; }

    // Device Network (Internet)
    public string DeviceNetworkName { get; set; }
    public string DeviceNetworkIp { get; set; }
    public string DeviceNetworkMac { get; set; }

    // Device Hardware (System)
    public string DeviceHardwareId { get; set; }
    public string DeviceHardwareMotherboardName { get; set; }
    public string DeviceHardwareOsName { get; set; }

    // Device Processor (CPU)
    public string DeviceProcessorName { get; set; }

    // Device Graphics (GPU)
    public string DeviceGraphicsName { get; set; }
}