namespace Coa.Auth.Api.DependencyInjections;

public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}