using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Liup.UserInteraction.Domain.Models.MongoModels;

[BsonIgnoreExtraElements]
public sealed class UserProperty : PropertyDeriveModel
{
    public sealed override Object? UserId
    {
        get => userId;
        set => userId = value;
    }
    private volatile Object? userId;

    public sealed override bool? IsEmailConfirmed
    {
        get => isEmailConfirmed;
        set => isEmailConfirmed = value;
    }
    private bool? isEmailConfirmed;

    public sealed override bool? IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }
    private bool? isLocked;

    public sealed override bool? IsDeleted
    {
        get => isDeleted;
        set => isDeleted = value;
    }
    private bool? isDeleted;

    public sealed override string? Token
    {
        get => token;
        set => token = value;
    }
    private volatile string? token;

    public sealed override string? CreatedDate
    {
        get => createdDate;
        set => createdDate = value;
    }
    private volatile string? createdDate;

    public sealed override string? ModifiedDate
    {
        get => modifiedDate;
        set => modifiedDate = value;
    }
    private volatile string? modifiedDate;
}

public class PropertyDeriveModel
{
    public PropertyDeriveModel() { }

    public PropertyDeriveModel(Object? userId, bool? isEmailConfirmed, bool? isLocked, bool? isDeleted, string? token, string? createdDate, string? modifiedDate)
    {
        UserId = userId;
        IsEmailConfirmed = isEmailConfirmed;
        IsLocked = isLocked;
        IsDeleted = isDeleted;
        Token = token;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }

    //[DataMember]
    [BsonElement("UserId")]
    public virtual Object? UserId { get; set; }

    [DataMember]
    [BsonElement("IsEmailConfirmed")]
    public virtual bool? IsEmailConfirmed { get; set; }

    [DataMember]
    [BsonElement("IsLocked")]
    public virtual bool? IsLocked { get; set; }

    [DataMember]
    [BsonElement("IsDeleted")]
    public virtual bool? IsDeleted { get; set; }

    [DataMember]
    [BsonElement("Token")]
    public virtual string? Token { get; set; }

    [DataMember]
    [BsonElement("CreatedDate")]
    public virtual string? CreatedDate { get; set; }

    [DataMember]
    [BsonElement("ModifiedDate")]
    public virtual string? ModifiedDate { get; set; }
}