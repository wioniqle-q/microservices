using Auth.Infrastructure.Data.MongoDB.ContextInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextBase;

public sealed class UserMongoContext(IBuildContext buildContext) : IContext
{
    private readonly SemaphoreSlim _sessionSemaphore = new(15, 20);

    public string DatabaseName => buildContext.DatabaseName;
    
    public IMongoCollection<T> GetCollection<T>(string name) 
    {
        return buildContext.GetCollection<T>(name);
    }

    public async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        await _sessionSemaphore.WaitAsync(cancellationToken);
        
        try 
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

            return await buildContext.Client.StartSessionAsync(sessionOptions, cancellationToken);
        }
        finally 
        {
            _sessionSemaphore.Release();
        }
    }
}

