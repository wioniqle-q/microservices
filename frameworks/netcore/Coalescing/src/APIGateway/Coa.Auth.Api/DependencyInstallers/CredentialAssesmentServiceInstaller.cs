using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using Auth.Infrastructure.UserTransaction.UserVerification;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class CredentialAssesmentServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CredentialAssesment>();
        services.AddScoped<ICredentialAssesment, CredentialAssesment>();
        services.AddScoped<IOutCome, OutcomeValue>();
        services.AddScoped<OutcomeValue>();
    }
}