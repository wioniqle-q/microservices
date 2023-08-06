using Auth.Infrastructure.AdlemanProtocol;
using Auth.Infrastructure.AdlemanProtocol.AdlemanAbstracts;
using Auth.Infrastructure.AdlemanProtocol.AdlemanInterfaces;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class AdlemanServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Adleman>();
        services.AddScoped<IAdleman, Adleman>();
        services.AddScoped<AdlemanAbstract, Adleman>();

        services.AddScoped<AdlemanIdentity>();
        services.AddScoped<IAdlemanIdentity, AdlemanIdentity>();
        services.AddScoped<AdlemanIdentityAbstract, AdlemanIdentity>();
    }
}