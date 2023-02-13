using Liup.UserInteraction.Infrastructure.Data.Interfaces;
using Liup.UserInteraction.Infrastructure.Data.Modules;
using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.Data.Contexts;

public class InteractionMongoDbContext : IInteractionMongoDbContext
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public InteractionMongoDbContext(InteractionMongoDbModule settings)
    {
        Client = settings.MongoClient;
        Database = settings.Database;
    }

    public string DatabaseName => "UserInteraction";
    public virtual IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
}
