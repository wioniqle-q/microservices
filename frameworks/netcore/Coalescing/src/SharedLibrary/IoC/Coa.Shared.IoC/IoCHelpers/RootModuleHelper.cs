using Coa.Shared.IoC.IoCAbstractions;
using Coa.Shared.IoC.IoCInterfaces;

namespace Coa.Shared.IoC.IoCHelpers;

public static class RootModuleHelper
{
    public static async Task<List<Type>> EnumerateAllModuleTypes(Type moduleType)
    {
        var typeOfModules = await GetRootComponentModulesAsync(moduleType);
        return typeOfModules.ToList();
    }

    private static async Task<List<Type>> GetModuleDependencies(Type moduleType)
    {
        RootModule.InspectRootModuleType(moduleType);

        var modules = new HashSet<Type>();
        var dependedFactories = moduleType.GetCustomAttributes(true)
            .OfType<IDependedFactory>();

        foreach (var depended in dependedFactories)
        {
            var dependedTypes = depended.GetDependedTypesAsync();
            modules.UnionWith(dependedTypes);
        }

        return await Task.FromResult(modules.ToList());
    }

    private static async Task AddRootComponentModulesAsync(ISet<Type> modules, Type moduleType)
    {
        RootModule.InspectRootModuleType(moduleType);

        if (modules.Add(moduleType))
        {
            var dependencies = await GetModuleDependencies(moduleType);
            await Task.WhenAll(dependencies.Select(dependedModule =>
                AddRootComponentModulesAsync(modules, dependedModule)));
        }
    }

    private static async Task<HashSet<Type>> GetRootComponentModulesAsync(Type moduleType)
    {
        RootModule.InspectRootModuleType(moduleType);

        var modules = new HashSet<Type>();
        await AddRootComponentModulesAsync(modules, moduleType);
        return modules;
    }
}