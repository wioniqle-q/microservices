using Coa.Shared.IoC.IoCAbstractions;
using Coa.Shared.IoC.IoCContexts;
using Coa.Shared.IoC.IoCHelpers;
using Coa.Shared.IoC.IoCInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coa.Shared.IoC.IoCModule;

public class ModuleRegistry : ModuleManagerFactoryAbstract
{
    private readonly IModuleLoaderFactory _moduleLoaderFactory;

    public ModuleRegistry(IModuleLoaderFactory moduleLoaderFactory)
    {
        _moduleLoaderFactory = moduleLoaderFactory;
    }

    public override async Task InitializeModuleAsync(IEnumerable<Type> moduleTypes)
    {
        var typeOfModules = moduleTypes
            .SelectMany(startupModuleType => RootModuleHelper.EnumerateAllModuleTypes(startupModuleType).Result)
            .ToList();

        var assemblies = typeOfModules.Select(module => module.Assembly).Distinct().ToList();

        var configurationContext = new ComponentConfigurationContext(new ServiceCollection());
        var modules = await _moduleLoaderFactory.LoadModulesAsync(assemblies, configurationContext)
            .ConfigureAwait(false);

        var tasks = modules.Select(module => StartModuleAsync(module, configurationContext));
        await Task.WhenAll(tasks);
    }

    public override async Task ShutdownModulesAsync(IEnumerable<Type> typeOfModules, IServiceProvider serviceProvider)
    {
        var modules = typeOfModules
            .SelectMany(moduleType => GetDependedModuleTypes(moduleType).Concat(new[] { moduleType }))
            .Distinct()
            .ToList();

        var sortedModules = ComponentHelper.SortByDependencies(modules.ToList(), GetDependedModuleTypes);

        var moduleTasks = sortedModules.Select(moduleType =>
        {
            var module = (IComponentModule)serviceProvider.GetRequiredService(moduleType);
            return module.ComponentShutdown(new ComponentShutdownContext(serviceProvider));
        });

        await Task.WhenAll(moduleTasks);
    }

    protected virtual async Task StartModuleAsync(IComponentModule module, ComponentConfigurationContext context)
    {
        var tasks = new HashSet<Task>
        {
            module.ServiceRegistration(),
            module.ComponentBoostrap(context),
            module.ComponentStartup()
        };

        await Task.WhenAll(tasks);
    }

    private static IEnumerable<Type> GetDependedModuleTypes(Type moduleType)
    {
        return moduleType
            .GetCustomAttributes(typeof(IDependedFactory), true)
            .Cast<IDependedFactory>()
            .SelectMany(descriptor => descriptor.GetDependedTypesAsync());
    }
}