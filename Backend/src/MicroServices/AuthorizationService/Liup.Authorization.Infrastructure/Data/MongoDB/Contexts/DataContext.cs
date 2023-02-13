using Liup.Authorization.Infrastructure.Data.MongoDB.Configurations;
using Liup.Authorization.Infrastructure.Data.MongoDB.Interfaces;
using MongoDB.Driver;

namespace Liup.Authorization.Infrastructure.Data.MongoDB.Contexts;
public sealed class DataContext : IDataContext
{
    private readonly IMongoClient _client;
    public IMongoDatabase Database { get; }

    public DataContext(DataConfiguration configurations)
    {
        _client = configurations.MongoClient;
        Database = configurations.Database;
    }

    public string DatabaseName => Database.DatabaseNamespace.DatabaseName;

    public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);

    public async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
    {
        var options = new ClientSessionOptions
        {
            CausalConsistency = true,
            DefaultTransactionOptions = new TransactionOptions(
                readConcern: ReadConcern.Majority,
                writeConcern: WriteConcern.WMajority)
        };

        return await _client.StartSessionAsync(options, cancellationToken);
    }
}