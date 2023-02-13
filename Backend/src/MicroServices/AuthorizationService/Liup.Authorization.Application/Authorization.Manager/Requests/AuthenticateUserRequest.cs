using MediatR;

namespace Liup.Authorization.Application.Authorization.Manager.Requests;

public sealed class AuthenticateUserRequest : IRequest<AuthenticateUserResult>
{
    public AuthenticateUserRequest(string? userName, string? firstName, string middleName, string lastName, string email, string password)
    {
        UserName = userName;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    public string? UserName { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
}