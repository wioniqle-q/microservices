using Auth.Domain.EntitiesAbstractions.SignatureEntitiesAbstractions;

namespace Auth.Domain.Entities.SignatureEntities;

public class BaseAccessSignatureEntitiy : BaseAccessSignatureAbstract
{
    public override string TransactionId { get; set; } = null!;
    public override bool TrialStatus { get; set; }
    public override string UserId { get; set; } = null!;
    public override string UserName { get; set; } = null!;
    public override string Email { get; set; } = null!;
    public override bool IsAccess { get; set; }
    public override string CustomAuthorization { get; set; } = null!;
    public override string OccurrenceTime { get; set; } = null!;
    public override string EnrollmentDate { get; set; } = null!;
}