using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.Data.MongoDB.ContextBase;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

public abstract class UserOperationAbstract : IUserOperation
{
    private readonly UserMongoContext _context;

    protected UserOperationAbstract(UserMongoContext context)
    {
        _context = context;
    }

    public virtual async Task<BaseUserEntitiy?> GetUserByIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);
        var options = new FindOptions<BaseUserEntitiy>
        {
            Limit = 1,
            AllowPartialResults = false
        };

        using var cursor = await collection.FindAsync(x => x.UserId == userId, options, cancellationToken);
        return (await cursor.FirstOrDefaultAsync(cancellationToken) ?? null)!;
    }

    public virtual async Task<BaseUserEntitiy> FindUserByQueryAsync(FilterDefinition<BaseUserEntitiy> query,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);
        var options = new FindOptions<BaseUserEntitiy>
        {
            Limit = 1,
            AllowPartialResults = false
        };

        using var cursor = await collection.FindAsync(query, options, cancellationToken);
        return (await cursor.FirstOrDefaultAsync(cancellationToken) ?? null)!;
    }

    public virtual async Task<bool> CreateUserAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(async (s, ct) =>
        {
            await collection.InsertOneAsync(s, user, cancellationToken: ct)
                .ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);

        return session;
    }

    public async Task<bool> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(async (s, ct) =>
        {
            await collection.InsertManyAsync(s, users, cancellationToken: ct)
                .ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);

        return session;
    }

    public virtual async Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(async (s, ct) =>
        {
            await collection.UpdateOneAsync(s, filter, update, cancellationToken: ct)
                .ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);

        return session;
    }

    private async Task<bool> StartSessionWithActionAsync(Func<IClientSessionHandle, CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        var session = await _context.StartSessionAsync(cancellationToken);
        session.StartTransaction();

        try
        {
            await action(session, cancellationToken).ConfigureAwait(false);
            await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }
        finally
        {
            session.Dispose();
        }
    }
}