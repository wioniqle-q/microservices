using Auth.Infrastructure.UserTransaction.Interfaces;
using Auth.Infrastructure.UserTransaction.UserInsertation;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class InsertAssesmentServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<InsertAssesment>();
        services.AddScoped<IInsertAssesment, InsertAssesment>();
    }
}