using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;

public abstract class BaseUserAbstract : IBaseUser
{
    public virtual Guid UserId { get; set; }
    public virtual string UserName { get; set; } = null!;
    public virtual string FirstName { get; set; } = null!;
    public virtual string MiddleName { get; set; } = null!;
    public virtual string LastName { get; set; } = null!;
    public virtual string Email { get; set; } = null!;
    public virtual string Password { get; set; } = null!;
    public virtual BaseUserProperty UserProperty { get; set; } = null!;
    public virtual BaseDevice Device { get; set; } = null!;
    public virtual BaseUserRsa UserRsa { get; set; } = null!;
}