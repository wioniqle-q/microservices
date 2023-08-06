using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MediatR;

namespace Auth.Application.InsertManager.Requests;

public sealed class InsertUserRequest : IRequest<OutcomeValue>
{
    public InsertUserRequest(string userName, string firstName, string middleName, string lastName, string email,
        string password, string timeZone, BaseDevice device)
    {
        UserName = userName;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        Password = password;
        TimeZone = timeZone;
        Device = device;
    }

    public string UserName { get; }
    public string FirstName { get; }
    public string MiddleName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string Password { get; }
    public string TimeZone { get; }
    public BaseDevice Device { get; }
}