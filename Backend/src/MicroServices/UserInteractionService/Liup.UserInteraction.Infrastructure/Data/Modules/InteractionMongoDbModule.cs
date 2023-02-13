using Liup.UserInteraction.Infrastructure.Data.Options;
using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.Data.Modules;

public class InteractionMongoDbModule
{
    public InteractionMongoDbModule(InteractionMongoDbSettings settings)
    {
        MongoClient = CreateMongoClient(new MongoUrl(settings.ConnectionString));
        Database = MongoClient.GetDatabase(settings.DatabaseName);
    }

    public IMongoClient MongoClient { get; }
    public IMongoDatabase Database { get; }

    protected virtual MongoClient CreateMongoClient(MongoUrl options)
    {
        var mongoClientSettings = MongoClientSettings.FromUrl(options);

        mongoClientSettings.MaxConnectionPoolSize = 100;
        mongoClientSettings.MinConnectionPoolSize = 10;

        mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        mongoClientSettings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);

        mongoClientSettings.ReadConcern = ReadConcern.Majority;
        mongoClientSettings.WriteConcern = WriteConcern.WMajority;

        return new MongoClient(mongoClientSettings);
    }
}

