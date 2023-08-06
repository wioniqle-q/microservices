using Auth.Domain.Entities.MongoEntities;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

public interface IUserOperation
{
    Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<BaseUserEntitiy> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default);

    Task<bool> CreateUserAsync(BaseUserEntitiy user, CancellationToken cancellationToken = default);

    Task<bool> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default);
}