using Auth.Infrastructure.Data.MongoDB.ContextBase;
using Auth.Infrastructure.Data.MongoDB.ContextBuilder;
using Auth.Infrastructure.Data.MongoDB.ContextInterfaces;
using Auth.Infrastructure.Data.MongoDB.ContextOption;
using Auth.Infrastructure.UserOperation.UserMongoLayer;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserHelpers;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class MongoDbServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var userContextOptions = CreateUserContextOptions(configuration);
        services.AddSingleton(userContextOptions);

        var buildContext = CreateBuildContext(userContextOptions);
        services.AddSingleton<IBuildContext>(buildContext);

        services.AddScoped<UserMongoContext>();
        services.AddScoped<IContext, UserMongoContext>();
        services.AddTransient<IBuildContext, BuildContextGeneric<UserMongoContext, UserContextOptions>>();

        services.AddTransient<UserOperation>();
        services.AddTransient<IUserOperation, UserOperation>();
        services.AddTransient<UserOperationAbstract, UserOperation>();

        services.AddTransient<UserHelper>();
        services.AddTransient<IUserHelper, UserHelper>();
        services.AddTransient<UserHelperAbstract, UserHelper>();

        services.AddScoped<StaticGetTimeZone>();
        services.AddTransient<IStaticGetTimeZone, StaticGetTimeZone>();
        services.AddTransient<StaticGetTimeZoneAbstract, StaticGetTimeZone>();
    }

    private static UserContextOptions CreateUserContextOptions(IConfiguration configuration)
    {
        return new UserContextOptions
        {
            ConnectionString = configuration.GetSection("MongoDb:ConnectionString").Value ?? string.Empty,
            DatabaseName = configuration.GetSection("MongoDb:DatabaseName").Value ?? string.Empty,
            CollectionName = configuration.GetSection("MongoDb:CollectionName").Value ?? string.Empty
        };
    }

    private static BuildContextGeneric<UserMongoContext, UserContextOptions> CreateBuildContext(
        UserContextOptions userContextOptions)
    {
        return BuildContextGeneric<UserMongoContext, UserContextOptions>.Instance(userContextOptions);
    }
}
