using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;
using Auth.Infrastructure.UserTransaction.UserVerification;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class CredentialAssesmentServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<CredentialAssesment>();
        services.AddTransient<ICredentialAssesment, CredentialAssesment>();
        services.AddTransient<IOutCome, OutcomeValue>();
        services.AddTransient<OutcomeValue>();
    }
}