namespace Auth.Domain.EntitiesInterfaces.MongoEntitiesInterfaces;

public interface IRsaUser
{
    string RsaPublicKey { get; set; }
    string RsaPrivateKey { get; set; }
    string RsaValidateKey { get; set; }
}