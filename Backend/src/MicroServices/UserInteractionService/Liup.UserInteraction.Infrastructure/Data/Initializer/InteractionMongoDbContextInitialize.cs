
using Liup.UserInteraction.Infrastructure.Data.Contexts;
using Liup.UserInteraction.Infrastructure.Data.Interfaces;
using Liup.UserInteraction.Infrastructure.Data.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Liup.UserInteraction.Infrastructure.Data.Initializer;

public static class InteractionMongoDbContextInitialize
{
    public static void AddInteractionMongoDbContextInitialize(this IServiceCollection services)
    {
        services.AddTransient<IInteractionMongoDbContextHelper, InteractionMongoDbContextHelper>();
        services.AddTransient<IInteractionMongoDbContext, InteractionMongoDbContext>();
        services.AddTransient<InteractionMongoDbModule>();
        services.AddTransient<InteractionMongoDbContextHelper>();
        services.AddTransient<InteractionMongoDbContext>();

        services.AddTransient<InteractionMongoDbContextHelper>();
    }
}
