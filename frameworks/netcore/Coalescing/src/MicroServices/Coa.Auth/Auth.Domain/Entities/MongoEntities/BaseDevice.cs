using System.Collections;
using Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Domain.Entities.MongoEntities;

[BsonIgnoreExtraElements]
public class BaseDevice : BaseDeviceAbstract
{
    public override Guid UserId { get; set; }
    public override string DeviceId { get; set; } = null!;
    public override string DeviceType { get; set; } = null!;

    public override string DeviceLocation { get; set; } = null!;
    public override string DeviceLocationTimeZone { get; set; } = null!;

    public override string DeviceNetworkName { get; set; } = null!;
    public override string DeviceNetworkIp { get; set; } = null!;
    public override string DeviceNetworkMac { get; set; } = null!;

    public override string DeviceHardwareId { get; set; } = null!;
    public override string DeviceHardwareMotherboardName { get; set; } = null!;
    public override string DeviceHardwareOsName { get; set; } = null!;

    public override string DeviceProcessorName { get; set; } = null!;
    public override string DeviceGraphicsName { get; set; } = null!;
}