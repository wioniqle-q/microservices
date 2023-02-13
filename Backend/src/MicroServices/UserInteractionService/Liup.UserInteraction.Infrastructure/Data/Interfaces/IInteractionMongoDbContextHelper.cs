using System.Linq.Expressions;
using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.Data.Interfaces;

public interface IInteractionMongoDbContextHelper
{
    Task<T> FindOneAsync<T>(Expression<Func<T, bool>> predicate, int limit, CancellationToken cancellationToken = default);
    Task<T> FindOneQueryAbleAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task InsertOneAsync<T>(T document, CancellationToken cancellationToken = default);
    Task<UpdateResult> UpdateOneAsync<T>(Expression<Func<T, bool>> predicate, UpdateDefinition<T> update, CancellationToken cancellationToken = default);

    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
}
