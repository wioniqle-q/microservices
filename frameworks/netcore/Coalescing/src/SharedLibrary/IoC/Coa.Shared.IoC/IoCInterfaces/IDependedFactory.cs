namespace Coa.Shared.IoC.IoCInterfaces;

public interface IDependedFactory
{
    IEnumerable<Type> GetDependedTypesAsync();
}