using System.Reflection;
using Coa.Shared.IoC.IoCAbstractions;
using Coa.Shared.IoC.IoCContexts;
using Coa.Shared.IoC.IoCInterfaces;

namespace Coa.Shared.IoC.IoCModule;

public class ModuleLoader : ModuleLoaderFactoryAbstract
{
    protected virtual async Task<List<IComponentModule>> ComponentModulesLoad(IEnumerable<Assembly> assemblies)
    {
        var modules = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IComponentModule).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => (IComponentModule)Activator.CreateInstance(type)!)
            .ToList();

        return await Task.FromResult(modules);
    }

    public override async Task<IEnumerable<IComponentModule>> LoadModulesAsync(IEnumerable<Assembly> startupModuleType,
        ComponentConfigurationContext context)
    {
        var modules = await ComponentModulesLoad(startupModuleType);
        return modules;
    }
}