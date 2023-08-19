using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.Data.MongoDB.ContextBase;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

public abstract class UserOperationAbstract : IUserOperation
{
    private readonly UserMongoContext _context;
    private readonly IMemoryCache _cache;
    
    protected UserOperationAbstract(UserMongoContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
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
    
    public async Task<List<BaseUserEntitiy>?> FindUsersByQueryWithPageAsync(int skip, int limit,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue($"Users_{skip}_{limit}", out List<BaseUserEntitiy>? cachedUsers))
        {
            return cachedUsers;
        }

        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var filter = Builders<BaseUserEntitiy>.Filter.Empty; 

        var options = new FindOptions<BaseUserEntitiy>
        {
            AllowPartialResults = false,
            Projection = Builders<BaseUserEntitiy>.Projection
                .Exclude("_id")
                .Exclude(x => x.Password)
                .Exclude(x => x.UserProperty)
                .Exclude(x => x.Device)
                .Exclude(x => x.LastName)
                .Exclude(x => x.MiddleName)
                .Exclude(x => x.UserRsa)
                .Exclude(x => x.FirstName),
            Limit = limit,
            Skip = skip
        };
        
        var result = new List<BaseUserEntitiy>();
        var fetchedCount = 0;

        using var cursor = await collection.FindAsync(filter, options, cancellationToken);

        var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        await cursor.ForEachAsync(async userEntity =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (fetchedCount >= skip && result.Count < limit)
                {
                    result.Add(userEntity);
                }
                fetchedCount++;
            }
            finally
            {
                semaphore.Release();
            }
        }, cancellationToken);

        if (result.Count >= limit)
        {
            result = result.Take(limit).ToList();
        }

        _cache.Set($"Users_{skip}_{limit}", result, TimeSpan.FromMinutes(10)); 

        return result;
    }


    public virtual async Task<bool> CreateUserAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(
            async (s, ct) => { await collection.InsertOneAsync(s, user, cancellationToken: ct); }, cancellationToken);

        return session;
    }

    public async Task<bool> CreateManyUserAsync(IEnumerable<BaseUserEntitiy> users,
        CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(
            async (s, ct) => { await collection.InsertManyAsync(s, users, cancellationToken: ct); }, cancellationToken);

        return session;
    }

    public virtual async Task<bool> UpdateUserAsync(FilterDefinition<BaseUserEntitiy> filter,
        UpdateDefinition<BaseUserEntitiy> update, CancellationToken cancellationToken = default)
    {
        var collection = _context.GetCollection<BaseUserEntitiy>(_context.DatabaseName);

        var session = await StartSessionWithActionAsync(async (s, ct) =>
        {
            await collection.UpdateOneAsync(s, filter, update, cancellationToken: ct)
                ;
        }, cancellationToken);

        return session;
    }

    private async Task<bool> StartSessionWithActionAsync(Func<IClientSessionHandle, CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        var session = await _context.StartSessionAsync(cancellationToken);
        session.StartTransaction();

        try
        {
            await action(session, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken);
            return false;
        }
        finally
        {
            session.Dispose();
        }
    }
}