using System.Reflection;

namespace Coa.Shared.IoC.IoCInterfaces;

public interface IModuleProviderFactory
{
    Assembly[] GetAssembliesAsync();
}