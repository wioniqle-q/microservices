using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.Data.Interfaces;

public interface IInteractionMongoDbContext
{
    IMongoClient Client { get; }

    IMongoDatabase Database { get; }
}
