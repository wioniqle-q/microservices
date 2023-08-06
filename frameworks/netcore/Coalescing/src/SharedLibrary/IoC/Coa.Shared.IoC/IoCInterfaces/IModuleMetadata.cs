namespace Coa.Shared.IoC.IoCInterfaces;

public interface IModuleMetadata
{
    Type ModuleType { get; }
    IComponentModule Instance { get; }
}