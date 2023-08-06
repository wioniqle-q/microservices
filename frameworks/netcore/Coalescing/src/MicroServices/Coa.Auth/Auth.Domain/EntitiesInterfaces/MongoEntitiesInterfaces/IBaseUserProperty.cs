namespace Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

public interface IBaseUserProperty
{
    Guid UserId { get; set; }
    bool IsEmailConfirmed { get; set; }
    bool IsLocked { get; set; }
    bool IsDeleted { get; set; }
    string? Token { get; set; }
    DateTime CreatedAt { get; set; }
    string LastLogin { get; set; }
    string TimeZone { get; set; }
    int LoginTimeSpan { get; set; }
    bool Require2Fa { get; set; }
    string DeviceId { get; set; }
    string AccessToken { get; set; }
    string RefreshToken { get; set; }
    List<string> RefreshTokens { get; set; }
}