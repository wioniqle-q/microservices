using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextInterfaces;

public interface IBuildContext
{
    public string DatabaseName { get; }
    public IMongoDatabase Database { get; }
    public IMongoClient Client { get; }
    public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null, 
        CancellationToken cancellationToken = default);
    public IMongoCollection<T> GetCollection<T>(string name);
}