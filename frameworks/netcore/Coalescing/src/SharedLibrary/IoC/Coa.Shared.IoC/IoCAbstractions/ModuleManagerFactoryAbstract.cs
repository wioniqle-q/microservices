using Coa.Shared.IoC.IoCInterfaces;

namespace Coa.Shared.IoC.IoCAbstractions;

public abstract class ModuleManagerFactoryAbstract : IModuleManagerFactory
{
    public abstract Task InitializeModuleAsync(IEnumerable<Type> moduleType);
    public abstract Task ShutdownModulesAsync(IEnumerable<Type> typeOfModules, IServiceProvider serviceProvider);
}