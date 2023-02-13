using MongoDB.Bson.Serialization.Attributes;

namespace Liup.Authorization.Domain.Models.MongoModels;

public class UserProperty
{
    private UserProperty() { }

    public UserProperty(string UserId, bool IsEmailConfirmed, bool IsLocked, bool IsDeleted, string Token, string CreatedDate)
    {
        this.UserId = Guid.Parse(UserId);
        this.IsEmailConfirmed = IsEmailConfirmed;
        this.IsLocked = IsLocked;
        this.IsDeleted = IsDeleted;
        this.Token = Token;
        this.CreatedDate = CreatedDate;
    }

    [BsonId]
    public virtual string? Id { get; private set; }

    [BsonElement("UserId")]
    public virtual Guid? UserId { get; private set; }

    [BsonElement("IsEmailConfirmed")]
    public virtual bool IsEmailConfirmed { get; private set; }

    [BsonElement("IsLocked")]
    public virtual bool IsLocked { get; private set; }

    [BsonElement("IsDeleted")]
    public virtual bool IsDeleted { get; private set; }

    [BsonElement("Token")]
    public virtual string? Token { get; private set; }

    [BsonElement("CreatedDate")]
    public virtual string? CreatedDate { get; private set; }
}
