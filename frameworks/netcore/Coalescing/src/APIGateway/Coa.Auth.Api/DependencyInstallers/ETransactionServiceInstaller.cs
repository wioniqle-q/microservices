using Auth.Infrastructure.EmailTransaction;
using Auth.Infrastructure.EmailTransaction.EmailTransactionAbstracts;
using Auth.Infrastructure.EmailTransaction.EmailTransactionInterfaces;
using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class ETransactionServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var eBaseOptions = new EBaseOptions(
            configuration["SmtpConfig:FromMail"] ?? string.Empty,
            configuration["SmtpConfig:FromMailPassword"] ?? string.Empty,
            configuration["SmtpConfig:SmtpServer"] ?? string.Empty,
            int.Parse(configuration["SmtpConfig:SmtpPort"] ?? string.Empty),
            bool.Parse(configuration["SmtpConfig:EnableSsl"] ?? string.Empty)
        );
        services.AddSingleton(eBaseOptions);

        services.AddTransient<EBaseTransaction>();
        services.AddTransient<IETransaction, EBaseTransaction>();
        services.AddTransient<EmailTransactionAbstract, EBaseTransaction>();
    }
}