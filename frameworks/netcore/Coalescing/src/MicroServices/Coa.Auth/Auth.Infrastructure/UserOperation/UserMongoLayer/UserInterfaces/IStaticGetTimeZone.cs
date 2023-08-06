namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

public interface IStaticGetTimeZone
{
    public Task<string> GetUserTimeZoneAsIp(string ip);
}