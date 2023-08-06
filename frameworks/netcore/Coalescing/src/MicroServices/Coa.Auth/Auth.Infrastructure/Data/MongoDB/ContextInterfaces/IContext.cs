using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextInterfaces;

public interface IContext
{
    string DatabaseName { get; }
    IMongoCollection<T> GetCollection<T>(string name);
    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
}