using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MediatR;

namespace Auth.Application.ResetPwManager.Requests;

public sealed class ResetPwUserRequest : IRequest<OutcomeValue>
{
    public ResetPwUserRequest(string userName, string oldPassword, string newPassword, string userTimeZone,
        BaseDevice device, string refreshToken, string accessToken)
    {
        UserName = userName;
        OldPassword = oldPassword;
        NewPassword = newPassword;
        UserTimeZone = userTimeZone;
        Device = device;
        RefreshToken = refreshToken;
        AccessToken = accessToken;
    }

    public string UserName { get; }
    public string OldPassword { get; }
    public string NewPassword { get; }
    public string RefreshToken { get; }
    public string AccessToken { get; }
    public string UserTimeZone { get; }
    public BaseDevice Device { get; }
}