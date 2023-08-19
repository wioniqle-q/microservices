using Auth.Infrastructure.UserTransaction.Interfaces;
using Auth.Infrastructure.UserTransaction.TransferVerification;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class TransferAssesmentServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TransferAssesment>();
        services.AddScoped<ITransferAssesment, TransferAssesment>();
        services.AddScoped<TransferAssesmentAbstract, TransferAssesment>();
    }
}