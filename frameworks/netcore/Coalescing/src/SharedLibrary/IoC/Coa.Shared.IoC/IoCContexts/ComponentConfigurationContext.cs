using Microsoft.Extensions.DependencyInjection;

namespace Coa.Shared.IoC.IoCContexts;

public sealed class ComponentConfigurationContext
{
    public ComponentConfigurationContext(IServiceCollection services)
    {
        Services = services;
        CustomData = new Dictionary<string, object>();
    }

    public IServiceCollection Services { get; }
    public IDictionary<string, object> CustomData { get; }
}