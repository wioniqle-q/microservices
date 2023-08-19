using Auth.Infrastructure.Data.MongoDB.ContextInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextAbstractions;

public abstract class BuildContextAbstract : IBuildContext
{
    public abstract string DatabaseName { get; }
    public abstract IMongoDatabase Database { get; }
    public abstract IMongoClient Client { get; }
    public abstract Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null, 
        CancellationToken cancellationToken = default);
    public abstract IMongoCollection<T> GetCollection<T>(string name);
}