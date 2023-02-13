namespace Liup.Authorization.Application.Authorization.Manager.Requests;

public sealed record class AuthenticateUserResult(string? Response)
{
    public string? Response { get; set; } = Response;
}