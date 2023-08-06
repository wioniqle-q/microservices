using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.Data.MongoDB.ContextBase;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer;

public sealed class UserOperation : UserOperationAbstract
{
    public UserOperation(UserMongoContext context) : base(context)
    {
    }

    public override async Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await base.GetUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return user;
    }

    public override async Task<BaseUserEntitiy> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default)
    {
        var user = await base.FindUserByQueryAsync(query, cancellationToken).ConfigureAwait(false);
        return user;
    }

    public override async Task<bool> CreateUserAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var result = await base.CreateUserAsync(user, cancellationToken).ConfigureAwait(false);
        return result;
    }

    public override async Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default)
    {
        var result = await base.UpdateUserAsync(filter, update, cancellationToken).ConfigureAwait(false);
        return result;
    }
}