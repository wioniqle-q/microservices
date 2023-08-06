using System.Reflection;
using Coa.Shared.IoC.IoCContexts;
using Coa.Shared.IoC.IoCInterfaces;

namespace Coa.Shared.IoC.IoCAbstractions;

public abstract class ModuleLoaderFactoryAbstract : IModuleLoaderFactory
{
    public abstract Task<IEnumerable<IComponentModule>> LoadModulesAsync(IEnumerable<Assembly> startupModuleType,
        ComponentConfigurationContext context);
}