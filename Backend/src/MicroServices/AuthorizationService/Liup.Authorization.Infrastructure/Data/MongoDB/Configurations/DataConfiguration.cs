using Liup.Authorization.Infrastructure.Data.MongoDB.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Liup.Authorization.Infrastructure.Data.MongoDB.Configurations;

public abstract class DataConfiguration
{
    public IMongoClient MongoClient { get; }
    public IMongoDatabase Database { get; }

    protected DataConfiguration(DataOptions dataOptions)
    {
        MongoClient = CreateMongoClient(new MongoUrl(dataOptions.ConnectionString));
        Database = MongoClient.GetDatabase(dataOptions.Database);
    }

    private IMongoClient CreateMongoClient(MongoUrl mongoUrl)
    {
        var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);

        mongoClientSettings.MaxConnectionPoolSize = 100;
        mongoClientSettings.MinConnectionPoolSize = 10;

        mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        mongoClientSettings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);

        mongoClientSettings.ReadConcern = ReadConcern.Majority;
        mongoClientSettings.WriteConcern = WriteConcern.WMajority;
        mongoClientSettings.LinqProvider = LinqProvider.V3;

        return new MongoClient(mongoUrl);
    }
}