using System.IO.Compression;
using Coa.Auth.Api.DependencyInjections;
using Microsoft.AspNetCore.ResponseCompression;
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
        services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

        services.AddResponseCompression(options => { options.EnableForHttps = false; });

        services.AddScoped<ILoggerFactory, LoggerFactory>();
        services.AddLogging();

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddControllers().AddNewtonsoftJson();
    }
}