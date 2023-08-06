using Auth.Infrastructure.ConcealmentProtocol;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class ConcealmentProtocolServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<BaseConcealment>();
        services.AddTransient<BaseConcealment, BaseConcealment>();
        services.AddTransient<IConcealment, BaseConcealment>();
    }
}