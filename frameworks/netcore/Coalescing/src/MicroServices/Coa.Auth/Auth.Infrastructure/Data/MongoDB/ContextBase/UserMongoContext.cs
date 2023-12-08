using Auth.Infrastructure.Data.MongoDB.ContextInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextBase;

public sealed class UserMongoContext(IBuildContext buildContext) : IContext
{
    private readonly object _lock = new();

    public string DatabaseName
    {
        get
        {
            lock (_lock)
            {
                return buildContext.DatabaseName;
            }
        }
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        lock (_lock)
        {
            return buildContext.GetCollection<T>(name);
        }
    }

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var sessionOptions = new ClientSessionOptions
            {
                CausalConsistency = true,
                DefaultTransactionOptions = new TransactionOptions(
                    ReadConcern.Majority,
                    writeConcern: WriteConcern.W2,
                    readPreference: ReadPreference.PrimaryPreferred,
                    maxCommitTime: new Optional<TimeSpan?>(TimeSpan.FromSeconds(60))
                )
            };
            
            return buildContext.StartSessionAsync(sessionOptions, cancellationToken);
        }
    }
}
