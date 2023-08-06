using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

public abstract class OutcomeValueAbstract : IOutCome
{
    public virtual string? UniqueResId { get; set; }
    public virtual string? Description { get; set; }
    public virtual string? SubTopic { get; set; }
    public virtual string? RsaPublicKey { get; set; }
    public virtual string? AccessToken { get; set; }
    public virtual string? RefreshToken { get; set; }
    public virtual string? ClientAccessToken { get; set; }
}