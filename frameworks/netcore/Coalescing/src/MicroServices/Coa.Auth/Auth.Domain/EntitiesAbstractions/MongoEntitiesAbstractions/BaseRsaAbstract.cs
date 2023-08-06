using Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

namespace Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;

public abstract class BaseRsaAbstract : IRsaUser
{
    public virtual string RsaPublicKey { get; set; } = null!;
    public virtual string RsaPrivateKey { get; set; } = null!;
    public virtual string RsaValidateKey { get; set; } = null!;
}