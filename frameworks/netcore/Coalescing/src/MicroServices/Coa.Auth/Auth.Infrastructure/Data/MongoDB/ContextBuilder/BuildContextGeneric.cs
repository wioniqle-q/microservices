using Auth.Infrastructure.Data.MongoDB.ContextAbstractions;
using Auth.Infrastructure.Data.MongoDB.ContextOption;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Auth.Infrastructure.Data.MongoDB.ContextBuilder;

public sealed class BuildContextGeneric<TContext, TOptions> : BuildContextAbstract where TOptions : UserContextOptions
{
    private static BuildContextGeneric<TContext, TOptions>? _buildContextGeneric;
    private readonly TOptions _contextOptions;

    public BuildContextGeneric(TOptions contextOptions)
    {
        _contextOptions = contextOptions;
        Client = CreateMongoClient(new MongoUrl(contextOptions.ConnectionString));
        Database = Client.GetDatabase(contextOptions.DatabaseName);
    }

    public override string DatabaseName => _contextOptions.CollectionName;
    public override IMongoDatabase Database { get; }
    public override IMongoClient Client { get; }

    public static BuildContextGeneric<TContext, TOptions> Instance(TOptions options)
    {
        return _buildContextGeneric ??= new BuildContextGeneric<TContext, TOptions>(options);
    }

    public override async Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null, CancellationToken cancellationToken = default)
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

        return await Client.StartSessionAsync(sessionOptions, cancellationToken);
    }

    public override IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }

    private static IMongoClient CreateMongoClient(MongoUrl url)
    {
        var mongoSettings = MongoClientSettings.FromUrl(url);

        mongoSettings.MaxConnectionPoolSize = 200;
        mongoSettings.MinConnectionPoolSize = 150;
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