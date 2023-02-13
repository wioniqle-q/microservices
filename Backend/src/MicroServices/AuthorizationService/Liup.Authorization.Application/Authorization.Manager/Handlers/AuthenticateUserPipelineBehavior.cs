using MediatR;

namespace Liup.Authorization.Application.Authorization.Manager.Handlers;

public sealed class AuthenticateUserPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Perform any pre-processing steps before the request handler is called
        var response = next();
        // Perform any post-processing steps after the request handler has been called
        return response;
    }
}