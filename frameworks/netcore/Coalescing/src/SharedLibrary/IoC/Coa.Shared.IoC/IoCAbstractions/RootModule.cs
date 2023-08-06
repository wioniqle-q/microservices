using Coa.Shared.IoC.IoCContexts;
using Coa.Shared.IoC.IoCInterfaces;

namespace Coa.Shared.IoC.IoCAbstractions;

public abstract class RootModule : IComponentModule
{
    public virtual Task ServiceRegistration()
    {
        return Task.CompletedTask;
    }

    public virtual Task ComponentBoostrap(ComponentConfigurationContext configurationContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task ComponentStartup()
    {
        return Task.CompletedTask;
    }

    public virtual Task ComponentShutdown(ComponentShutdownContext shutdownContext)
    {
        return Task.CompletedTask;
    }

    public IEnumerable<IComponentModule> ComponentModules => null!;

    public static void InspectRootModuleType(Type type)
    {
        if (!typeof(IComponentModule).IsAssignableFrom(type))
            throw new ArgumentException(
                $"Given type is not an {nameof(IComponentModule)}: {type.AssemblyQualifiedName}");
    }
}