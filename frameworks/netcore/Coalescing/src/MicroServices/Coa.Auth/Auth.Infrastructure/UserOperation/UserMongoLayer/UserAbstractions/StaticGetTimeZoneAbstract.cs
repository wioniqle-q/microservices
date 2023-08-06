using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

public abstract class StaticGetTimeZoneAbstract : IStaticGetTimeZone
{
    public abstract Task<string> GetUserTimeZoneAsIp(string ip);
}