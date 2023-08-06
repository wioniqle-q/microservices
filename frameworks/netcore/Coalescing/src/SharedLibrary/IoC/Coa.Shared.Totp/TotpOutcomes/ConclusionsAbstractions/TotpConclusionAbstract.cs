using Coa.Shared.Totp.TotpOutcomes.ConclusionsInterfaces;

namespace Coa.Shared.Totp.TotpOutcomes.ConclusionsAbstractions;

public abstract class TotpConclusionAbstract : ITotpConclusion
{
    public virtual int? UniqueStatusCode { get; set; }
    public virtual int? TotpCode { get; set; }
    public virtual string? Description { get; set; }
}