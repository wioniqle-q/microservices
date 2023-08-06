using Coa.Auth.Api.DependencyInjections;
using Coa.Shared.Totp.TotpAbstractions;
using Coa.Shared.Totp.TotpAccessors;
using Coa.Shared.Totp.TotpInterfaces;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class TotpServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<TotpBase>();
        services.AddTransient<ITotp, TotpBase>();
        services.AddTransient<TotpAbstract, TotpBase>();
    }
}