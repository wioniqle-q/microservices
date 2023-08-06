using Auth.Domain.Entities.MongoEntities;

namespace Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

public interface IBaseUser
{
    Guid UserId { get; set; }
    string UserName { get; set; }
    string FirstName { get; set; }
    string MiddleName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    string Password { get; set; }
    BaseUserProperty UserProperty { get; set; }
    BaseDevice Device { get; set; }
    BaseUserRsa UserRsa { get; set; }
}