using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MediatR;

namespace Auth.Application.AuthManager.Requests;

public record AuthUserRequest : IRequest<OutcomeValue>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string UserTimeZone { get; set; } = null!;
    public BaseDevice Device { get; set; } = null!;
}