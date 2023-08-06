using Coa.Auth.Api.DependencyInjections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class ApiModuleInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

        services.Configure<KestrelServerOptions>(options => { options.AddServerHeader = false; });

        services.AddTransient<ILoggerFactory, LoggerFactory>();
        services.AddLogging();

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddControllers();
    }
}