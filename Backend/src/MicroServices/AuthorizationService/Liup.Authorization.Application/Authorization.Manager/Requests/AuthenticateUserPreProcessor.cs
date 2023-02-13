using MediatR.Pipeline;

namespace Liup.Authorization.Application.Authorization.Manager.Requests;

public sealed class AuthenticateUserPreProcessor : IRequestPreProcessor<AuthenticateUserRequest>
{
    public Task Process(AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        // TODO: Perform any pre-processing tasks here, such as logging or data preparation
        return Task.CompletedTask;
    }
}