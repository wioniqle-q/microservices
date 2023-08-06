using Auth.Infrastructure.Data.MongoDB.ContextInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextBase;

public sealed class UserMongoContext : IContext
{
    private readonly IBuildContext _buildContext;

    public UserMongoContext(IBuildContext buildContext)
    {
        _buildContext = buildContext;
    }

    public string DatabaseName => _buildContext.DatabaseName;

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _buildContext.GetCollection<T>(name);
    }

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        return _buildContext.StartSessionAsync(cancellationToken);
    }
}