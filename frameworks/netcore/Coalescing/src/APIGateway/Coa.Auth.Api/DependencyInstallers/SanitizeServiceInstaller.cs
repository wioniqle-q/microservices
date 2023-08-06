using Auth.Infrastructure.SanitizeProtocol;
using Auth.Infrastructure.SanitizeProtocol.SanitizeAbstractions;
using Auth.Infrastructure.SanitizeProtocol.SanitizeInterfaces;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class SanitizeServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<BaseSanitize>();
        services.AddTransient<ISanitize, BaseSanitize>();
        services.AddTransient<SanitizeAbstract, BaseSanitize>();
    }
}