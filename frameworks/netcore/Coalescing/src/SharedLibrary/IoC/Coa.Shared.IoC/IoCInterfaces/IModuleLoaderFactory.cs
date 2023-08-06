using System.Reflection;
using Coa.Shared.IoC.IoCContexts;

namespace Coa.Shared.IoC.IoCInterfaces;

public interface IModuleLoaderFactory
{
    Task<IEnumerable<IComponentModule>> LoadModulesAsync(IEnumerable<Assembly> startupModuleType,
        ComponentConfigurationContext context);
}