namespace Coa.Shared.IoC.IoCInterfaces;

public interface IModuleManagerFactory
{
    Task InitializeModuleAsync(IEnumerable<Type> moduleType);
    Task ShutdownModulesAsync(IEnumerable<Type> typeOfModules, IServiceProvider serviceProvider);
}