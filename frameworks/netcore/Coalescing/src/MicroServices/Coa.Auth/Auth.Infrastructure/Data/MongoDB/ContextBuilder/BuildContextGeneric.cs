using Auth.Infrastructure.Data.MongoDB.ContextAbstractions;
using Auth.Infrastructure.Data.MongoDB.ContextOption;
using MongoDB.Driver;

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

    public override IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }

    public override async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        var sessionOptions = new ClientSessionOptions
        {
            CausalConsistency = true,
            DefaultTransactionOptions = new TransactionOptions(
                ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority,
                readPreference: ReadPreference.Primary
            )
        };

        return await Client.StartSessionAsync(sessionOptions, cancellationToken);
    }

    private static IMongoClient CreateMongoClient(MongoUrl url)
    {
        var mongoSettings = MongoClientSettings.FromUrl(url);

        mongoSettings.MaxConnectionPoolSize = 5;
        mongoSettings.MinConnectionPoolSize = 3;
        mongoSettings.ConnectTimeout = new TimeSpan(0, 0, 30);
        mongoSettings.SocketTimeout = new TimeSpan(0, 0, 30);
        mongoSettings.ServerSelectionTimeout = new TimeSpan(0, 0, 30);
        mongoSettings.WaitQueueTimeout = new TimeSpan(0, 0, 30);

        mongoSettings.MaxConnectionIdleTime = new TimeSpan(0, 0, 30);
        mongoSettings.MaxConnectionLifeTime = new TimeSpan(0, 0, 30);
        mongoSettings.HeartbeatInterval = new TimeSpan(0, 0, 30);
        mongoSettings.HeartbeatTimeout = new TimeSpan(0, 0, 30);
        mongoSettings.LocalThreshold = new TimeSpan(0, 0, 30);

        mongoSettings.ReadConcern = ReadConcern.Snapshot;
        mongoSettings.ReadPreference = ReadPreference.Primary;
        mongoSettings.WriteConcern = WriteConcern.WMajority;

        mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1, true);

        mongoSettings.Credential = MongoCredential.CreateCredential(
            url.DatabaseName,
            url.Username,
            url.Password
        );

        return new MongoClient(mongoSettings);
    }
}