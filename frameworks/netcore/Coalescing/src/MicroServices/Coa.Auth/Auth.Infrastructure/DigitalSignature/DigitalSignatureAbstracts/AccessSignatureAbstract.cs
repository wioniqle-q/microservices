using System.Security.Claims;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatureAbstracts;

public abstract class AccessSignatureAbstract : IAccessSignature
{
    public abstract Task<string> GenerateAccessToken(string? userId, DateTime expirationDate,
        ClaimsIdentity claimsIdentity,
        BaseAccessSignatureEntitiy? baseUserEntitiy);

    public abstract Task<bool> ValidateAccessTokenLifeTime(string? userToken);
}