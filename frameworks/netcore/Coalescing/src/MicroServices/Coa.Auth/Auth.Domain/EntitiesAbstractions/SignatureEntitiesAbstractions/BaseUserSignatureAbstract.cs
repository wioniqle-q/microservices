using Auth.Domain.EntitiesInterfaces.SignatureEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.SignatureEntitiesAbstractions;

public abstract class BaseUserSignatureAbstract : IBaseUserSignature
{
    public virtual string TransactionId { get; set; } = null!;
    public virtual bool TrialStatus { get; set; } = true;
    public virtual bool IsAuthorized { get; set; }
    public virtual string CustomAuthorization { get; set; } = null!;
    public virtual bool IsBlocked { get; set; }
    public virtual string OccurrenceTime { get; set; } = null!;
    public virtual string EnrollmentDate { get; set; } = null!;
    public virtual DateTime TrialDate { get; set; } = default;
}