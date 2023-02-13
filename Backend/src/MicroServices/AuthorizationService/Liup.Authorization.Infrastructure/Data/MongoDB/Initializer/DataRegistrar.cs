
using Liup.Authorization.Infrastructure.Data.MongoDB.Contexts;
using Liup.Authorization.Infrastructure.Data.MongoDB.Interfaces;
using Liup.Authorization.Infrastructure.Data.MongoDB.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liup.Authorization.Infrastructure.Data.MongoDB.Initializer;

public static class DataRegistrar
{
    public static void RegisterData(this IServiceCollection services)
    {
        services.AddTransient<DataOptions>();
        services.AddSingleton<IDataContext, DataContext>();
    }
}
