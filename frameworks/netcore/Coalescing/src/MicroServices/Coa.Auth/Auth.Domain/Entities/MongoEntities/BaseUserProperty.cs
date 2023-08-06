using Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Domain.Entities.MongoEntities;

[BsonIgnoreExtraElements]
public sealed class BaseUserProperty : BaseUserPropertyAbstract
{
    public override Guid UserId { get; set; }
    public override bool IsEmailConfirmed { get; set; }
    public override bool IsLocked { get; set; }
    public override bool IsDeleted { get; set; }
    public override string? Token { get; set; }
    public override DateTime CreatedAt { get; set; }
    public override string LastLogin { get; set; } = null!;
    public override string TimeZone { get; set; } = null!;
    public override int LoginTimeSpan { get; set; }
    public override bool Require2Fa { get; set; }
    public override string DeviceId { get; set; } = null!;
    public override string AccessToken { get; set; } = null!;
    public override string RefreshToken { get; set; } = null!;
    public override List<string> RefreshTokens { get; set; } = null!;
}