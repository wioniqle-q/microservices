using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using MediatR;

namespace Auth.Application.TransferManager.Requests;

public record TransferUserRequest : IRequest<OutcomeValue>
{
    public string RefreshToken { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
}