using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Liup.UserInteraction.Domain.Models.MongoModels;

[BsonIgnoreExtraElements]
public sealed class UserModel : DeriveModel
{
    public sealed override Object? UserId
    {
        get => userId;
        set => userId = value;
    }
    private volatile Object? userId;

    public sealed override string? UserName
    {
        get => userName;
        set => userName = value;
    }
    private volatile string? userName;

    public sealed override string? FirstName
    {
        get => firstName;
        set => firstName = value;
    }
    private volatile string? firstName;

    public sealed override string? MiddleName
    {
        get => middleName;
        set => middleName = value;
    }
    private volatile string? middleName;

    public sealed override string? LastName
    {
        get => lastName;
        set => lastName = value;
    }
    private volatile string? lastName;

    public sealed override string? Email
    {
        get => email;
        set => email = value;
    }
    private volatile string? email;

    public sealed override string? Password
    {
        get => password;
        set => password = value;
    }
    private volatile string? password;

    [NotNull]
    public sealed override UserProperty? UserProperties { get; set; }
}

public class DeriveModel
{
    public DeriveModel() { }

    public DeriveModel(Object? userId, string? userName, string? firstName, string? middleName, string? lastName, string? email, string? password)
    {
        UserId = userId;
        UserName = userName;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    [DataMember]
    [BsonElement("UserId")]
    public virtual Object? UserId { get; set; }

    [DataMember]
    [BsonElement("UserName")]
    public virtual string? UserName { get; set; }

    [DataMember]
    [BsonElement("FirstName")]
    public virtual string? FirstName { get; set; }

    [DataMember]
    [BsonElement("MiddleName")]
    public virtual string? MiddleName { get; set; }

    [DataMember]
    [BsonElement("LastName")]
    public virtual string? LastName { get; set; }

    [DataMember]
    [BsonElement("Email")]
    public virtual string? Email { get; set; }

    [DataMember]
    [BsonElement("Password")]
    public virtual string? Password { get; set; }

    [NotNull]
    [DataMember]
    [BsonElement("UserProperties")]
    public virtual UserProperty? UserProperties { get; set; }
}