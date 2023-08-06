using Coa.Auth.Api.DependencyInjections;

namespace Coa.Auth.Api.DependencyInstallers;

public sealed class ValidationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        /*
        var assemblies = new[]
        {
            typeof(AuthUserByIdValidator).Assembly,
            typeof(InsertUserByIdValidator).Assembly,
            typeof(ResetPwUserByIdValidator).Assembly
        };

        services.AddFluentValidation(assemblies);
        */
    }
}