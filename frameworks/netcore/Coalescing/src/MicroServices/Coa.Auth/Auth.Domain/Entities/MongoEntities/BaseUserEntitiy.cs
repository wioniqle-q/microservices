using Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Domain.Entities.MongoEntities;

[BsonIgnoreExtraElements]
public sealed class BaseUserEntitiy : BaseUserAbstract
{
    public override Guid UserId { get; set; }
    public override string UserName { get; set; } = null!;
    public override string FirstName { get; set; } = null!;
    public override string MiddleName { get; set; } = null!;
    public override string LastName { get; set; } = null!;
    public override string Email { get; set; } = null!;
    public override string Password { get; set; } = null!;
    public override BaseUserProperty UserProperty { get; set; } = null!;
    public override BaseDevice Device { get; set; } = null!;
    public override BaseUserRsa UserRsa { get; set; } = null!;
}