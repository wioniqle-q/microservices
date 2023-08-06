using Coa.Shared.IoC.IoCContexts;

namespace Coa.Shared.IoC.IoCInterfaces;

public interface IComponentModule
{
    IEnumerable<IComponentModule> ComponentModules { get; }
    Task ServiceRegistration();
    Task ComponentBoostrap(ComponentConfigurationContext configurationContext);
    Task ComponentStartup();
    Task ComponentShutdown(ComponentShutdownContext shutdownContext);
}