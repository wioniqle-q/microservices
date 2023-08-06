using Coa.Shared.Totp.TotpOutcomes.ConclusionsAbstractions;

namespace Coa.Shared.Totp.TotpOutcomes.ConclusionsValue;

public sealed class TotpValueConclusion : TotpConclusionAbstract
{
    public TotpValueConclusion(int? uniqueStatusCode = null, int? totpCode = null, string? description = null)
    {
        UniqueStatusCode = uniqueStatusCode;
        TotpCode = totpCode;
        Description = description;
    }

    public override int? UniqueStatusCode { get; set; }
    public override int? TotpCode { get; set; }
    public override string? Description { get; set; }
}    
