using Auth.Domain.EntitiesInterfaces.SignatureEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.SignatureEntitiesAbstractions;

public abstract class BaseAccessSignatureAbstract : IBaseAccessSignature
{
    public virtual string TransactionId { get; set; } = null!;
    public virtual bool TrialStatus { get; set; }
    public virtual string UserId { get; set; } = null!;
    public virtual string UserName { get; set; } = null!;
    public virtual string Email { get; set; } = null!;
    public virtual bool IsAccess { get; set; }
    public virtual string CustomAuthorization { get; set; } = null!;
    public virtual string OccurrenceTime { get; set; } = null!;
    public virtual string EnrollmentDate { get; set; } = null!;
}