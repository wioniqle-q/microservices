using System.Reflection;

namespace Coa.Auth.Api.DependencyInjections;

public static class DependencyInjection
{
    public static void InstallServices(this IServiceCollection services, IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var installers = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var installer in installers) installer.Install(services, configuration);
    }
}