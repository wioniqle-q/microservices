using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsInterfaces;

namespace Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsAbstractions;

public abstract class ConclusionsAbstract : IConclusions
{
    public virtual int? UniqueStatusCode { get; set; }
    public virtual string? Description { get; set; }
    public virtual string? UniqueInfo { get; set; }
    public virtual string? ExceptionId { get; set; }
    public virtual string? ExceptionType { get; set; }
    public virtual string? ExceptionMessage { get; set; }
    public virtual string? Token { get; set; }
    public virtual string? RefreshToken { get; set; }
}