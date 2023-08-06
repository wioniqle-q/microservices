using Auth.Infrastructure.UserTransaction.Interfaces;
using Auth.Infrastructure.UserTransaction.UserPwResetation;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class ResetPwAssesmentServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ResetAssesment>();
        services.AddTransient<IResetAssesment, ResetAssesment>();
    }
}