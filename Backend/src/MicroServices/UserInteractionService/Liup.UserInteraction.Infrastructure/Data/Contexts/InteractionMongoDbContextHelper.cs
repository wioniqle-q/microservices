using System.Linq.Expressions;
using Liup.UserInteraction.Infrastructure.Data.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Liup.UserInteraction.Infrastructure.Data.Contexts;

public class InteractionMongoDbContextHelper : IInteractionMongoDbContextHelper
{
    private readonly InteractionMongoDbContext @Context;

    public InteractionMongoDbContextHelper(InteractionMongoDbContext context)
    {
        @Context = context;
    }

    protected virtual IMongoQueryable<T> Queryable<T>() => @Context.GetCollection<T>(@Context.DatabaseName).AsQueryable();

    public virtual async Task<T> FindOneQueryAbleAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await this.Queryable<T>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<T> FindOneAsync<T>(Expression<Func<T, bool>> predicate, int limit = 1, CancellationToken cancellationToken = default)
    {
        return await @Context.GetCollection<T>(@Context.DatabaseName).Find(predicate).Limit(limit).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task InsertOneAsync<T>(T document, CancellationToken cancellationToken = default)
    {
        await @Context.GetCollection<T>(@Context.DatabaseName).InsertOneAsync(document, cancellationToken: cancellationToken);
    }
    
    public virtual async Task<UpdateResult> UpdateOneAsync<T>(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update, CancellationToken cancellationToken = default)
    {
        return await @Context.GetCollection<T>(@Context.DatabaseName).UpdateOneAsync(predicate, update, cancellationToken: cancellationToken);
    }

    public virtual async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        var options = new ClientSessionOptions
        {
            CausalConsistency = true,
            DefaultTransactionOptions = new TransactionOptions(
                readConcern: ReadConcern.Majority,
                writeConcern: WriteConcern.WMajority)
        };

        return await @Context.Client.StartSessionAsync(options, cancellationToken);
    }
}
