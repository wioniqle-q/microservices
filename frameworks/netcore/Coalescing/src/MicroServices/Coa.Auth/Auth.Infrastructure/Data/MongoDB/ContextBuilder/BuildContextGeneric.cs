using Auth.Infrastructure.Data.MongoDB.ContextAbstractions;
using Auth.Infrastructure.Data.MongoDB.ContextOption;
using MongoDB.Driver;

namespace Auth.Infrastructure.Data.MongoDB.ContextBuilder;

public sealed class BuildContextGeneric<TContext, TOptions>(TOptions contextOptions) : BuildContextAbstract
    where TOptions : UserContextOptions
{
    private static readonly object Lock = new();
    private static BuildContextGeneric<TContext, TOptions>? _buildContextGeneric;
    private readonly IMongoClient _client = CreateMongoClient(new MongoUrl(contextOptions.ConnectionString));

    public override string DatabaseName => contextOptions.CollectionName;

    public override IMongoDatabase Database
    {
        get
        {
            lock (Lock)
            {
                return _client.GetDatabase(contextOptions.DatabaseName);
            }
        }
    }

    public override IMongoClient Client
    {
        get
        {
            lock (Lock)
            {
                return _client;
            }
        }
    }

    public static BuildContextGeneric<TContext, TOptions> Instance(TOptions options)
    {
        lock (Lock)
        {
            return _buildContextGeneric ??= new BuildContextGeneric<TContext, TOptions>(options);
        }
    }

    public override Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null, CancellationToken cancellationToken = default)
    {
        lock (Lock)
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

            return _client.StartSessionAsync(options ?? sessionOptions, cancellationToken: cancellationToken);
        }
    }

    public override IMongoCollection<T> GetCollection<T>(string name)
    {
        lock (Lock)
        {
            return Database.GetCollection<T>(name);
        }
    }

    private static IMongoClient CreateMongoClient(MongoUrl url)
    {
        var mongoSettings = MongoClientSettings.FromUrl(url);

        mongoSettings.MaxConnectionPoolSize = 35;
        mongoSettings.MinConnectionPoolSize = 20;
        mongoSettings.ConnectTimeout = new TimeSpan(0, 0, 10);
        mongoSettings.SocketTimeout = new TimeSpan(0, 0, 15);
        mongoSettings.ServerSelectionTimeout = new TimeSpan(0, 0, 15);
        mongoSettings.WaitQueueTimeout = new TimeSpan(0, 0, 15);

        mongoSettings.MaxConnectionIdleTime = new TimeSpan(0, 0, 30);
        mongoSettings.MaxConnectionLifeTime = new TimeSpan(0, 0, 30);
        mongoSettings.HeartbeatInterval = new TimeSpan(0, 0, 120);
        mongoSettings.HeartbeatTimeout = new TimeSpan(0, 0, 120);
        mongoSettings.LocalThreshold = new TimeSpan(0, 0, 60);

        mongoSettings.ReadConcern = ReadConcern.Majority;
        mongoSettings.ReadPreference = ReadPreference.SecondaryPreferred;
        mongoSettings.WriteConcern = WriteConcern.W2;
        
        mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1, true);

        return new MongoClient(mongoSettings);
    }
}
