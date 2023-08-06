using Auth.Domain.EntitiesAbstractions.SignatureEntitiesAbstractions;

namespace Auth.Domain.Entities.SignatureEntities;

public sealed class BaseUserSignatureEntitiy : BaseUserSignatureAbstract
{
    public override string TransactionId { get; set; } = null!;
    public override bool TrialStatus { get; set; } = true;
    public override bool IsAuthorized { get; set; }
    public override string CustomAuthorization { get; set; } = null!;
    public override bool IsBlocked { get; set; }
    public override string OccurrenceTime { get; set; } = null!;
    public override string EnrollmentDate { get; set; } = null!;
    public override DateTime TrialDate { get; set; }
}