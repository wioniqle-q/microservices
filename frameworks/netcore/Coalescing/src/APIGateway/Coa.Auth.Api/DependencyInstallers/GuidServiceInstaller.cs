using Auth.Infrastructure.GuidProtocol;
using Auth.Infrastructure.GuidProtocol.GuidAbstractions;
using Auth.Infrastructure.GuidProtocol.GuidInterfaces;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class GuidServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<BaseGuid>();
        services.AddTransient<IGuid, BaseGuid>();
        services.AddTransient<GuidAbstract, BaseGuid>();
    }
}