using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.ConclusionsAbstraction;

public abstract class OutcomeValueAbstract : IOutCome
{
    public virtual string? Outcome { get; set; }
    public virtual string? UniqueResId { get; set; }
    public virtual string? Description { get; set; }
    public virtual string? SubTopic { get; set; }
    public virtual string? RsaPublicKey { get; set; }
    public virtual string? AccessToken { get; set; }
    public virtual string? RefreshToken { get; set; }
    public virtual string? ClientAccessToken { get; set; }
    public virtual string? DeviceId { get; set; }
    public virtual string? ExceptionId { get; set; }
    public virtual string? ExceptionType { get; set; }
}