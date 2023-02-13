using MongoDB.Bson.Serialization.Attributes;

namespace Liup.Authorization.Domain.Models.MongoModels;

public abstract class UserModelAssesment
{
    public virtual string? UserName { get; set; }
    public virtual string? FirstName { get; set; }
    public virtual string? MiddleName { get; set; }
    public virtual string? LastName { get; set; }
    public virtual string? Email { get; set; }
    public virtual string? Password { get; set; }
}

public class UserModel : UserModelAssesment
{
    private UserModel() { }

    public UserModel(string UserName, string FirstName, string MiddleName, string LastName, string Email, string Password)
    {
        this.UserName = UserName;
        this.FirstName = FirstName;
        this.MiddleName = MiddleName;
        this.LastName = LastName;
        this.Email = Email;
        this.Password = Password;
    }

    [BsonId]
    public virtual string? Id { get; private set; }

    [BsonElement("UserId")]
    public virtual Guid? UserId { get; private set; } = Guid.NewGuid();

    public sealed override string? UserName
    {
        get => @userName;
        set => @userName = value;
    }
    private volatile string? @userName;

    public sealed override string? FirstName
    {
        get => @firstName;
        set => @firstName = value;
    }
    private volatile string? @firstName;

    public sealed override string? MiddleName
    {
        get => @middleName;
        set => @middleName = value;
    }
    private volatile string? @middleName;

    public sealed override string? LastName
    {
        get => @lastName;
        set => @lastName = value;
    }
    private volatile string? @lastName;

    public sealed override string? Email
    {
        get => @email;
        set => @email = value;
    }
    private volatile string? @email;

    public sealed override string? Password
    {
        get => @password;
        set => @password = value;
    }
    private volatile string? @password;
}