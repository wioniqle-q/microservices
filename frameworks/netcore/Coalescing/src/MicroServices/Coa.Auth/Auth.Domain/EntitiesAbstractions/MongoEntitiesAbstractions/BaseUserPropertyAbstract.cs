using Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;

public abstract class BaseUserPropertyAbstract : IBaseUserProperty
{
    public virtual Guid UserId { get; set; }
    public virtual bool IsEmailConfirmed { get; set; }
    public virtual bool IsLocked { get; set; }
    public virtual bool IsDeleted { get; set; }
    public virtual string? Token { get; set; } = null!;
    public virtual DateTime CreatedAt { get; set; }
    public virtual string LastLogin { get; set; } = null!;
    public virtual int LoginTimeSpan { get; set; }
    public virtual string TimeZone { get; set; } = null!;
    public virtual bool Require2Fa { get; set; }
    public virtual string DeviceId { get; set; } = null!;
    public virtual string AccessToken { get; set; } = null!;
    public virtual string RefreshToken { get; set; } = null!;
    public virtual List<string> RefreshTokens { get; set; } = null!;
}