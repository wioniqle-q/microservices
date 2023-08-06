using Auth.Domain.EntitiesAbstractions.MongoEntitiesAbstractions;

namespace Auth.Domain.Entities.MongoEntities;

public sealed class BaseUserRsa : BaseRsaAbstract
{
    public override string RsaPublicKey { get; set; } = null!;
    public override string RsaPrivateKey { get; set; } = null!;
    public override string RsaValidateKey { get; set; } = null!;
}