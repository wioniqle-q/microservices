namespace Coa.Shared.IoC.IoCContexts;

public sealed class ComponentShutdownContext
{
    public ComponentShutdownContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }
}