using MongoDB.Driver;

namespace Liup.Authorization.Infrastructure.Data.MongoDB.Interfaces;

public interface IDataContext
{
    IMongoDatabase Database { get; }
    IMongoCollection<T> GetCollection<T>(string name);
    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
}

