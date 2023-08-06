﻿using Auth.Infrastructure.Data.MongoDB.ContextBase;
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
        var userContextOptions = new UserContextOptions
        {
            ConnectionString = configuration.GetSection("MongoDb:ConnectionString").Value ?? string.Empty,
            DatabaseName = configuration.GetSection("MongoDb:DatabaseName").Value ?? string.Empty,
            CollectionName = configuration.GetSection("MongoDb:CollectionName").Value ?? string.Empty
        };
        services.AddSingleton(userContextOptions);

        var buildContext =
            BuildContextGeneric<UserMongoContext, UserContextOptions>.Instance(userContextOptions);
        services.AddSingleton<IBuildContext>(buildContext);

        services.AddTransient<UserMongoContext>();
        services.AddTransient<IContext, UserMongoContext>();
        services
            .AddTransient<IBuildContext,
                BuildContextGeneric<UserMongoContext, UserContextOptions>>();

        services.AddTransient<UserOperation>();
        services.AddTransient<IUserOperation, UserOperation>();
        services.AddTransient<UserOperationAbstract, UserOperation>();

        services.AddTransient<UserHelper>();
        services.AddTransient<IUserHelper, UserHelper>();
        services.AddTransient<UserHelperAbstract, UserHelper>();

        services.AddTransient<StaticGetTimeZone>();
        services.AddTransient<IStaticGetTimeZone, StaticGetTimeZone>();
        services.AddTransient<StaticGetTimeZoneAbstract, StaticGetTimeZone>();
    }
}