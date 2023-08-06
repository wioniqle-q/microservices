using System.Security.Claims;
using Auth.Domain.Entities.SignatureEntities;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;

public interface IAccessSignature
{
    Task<string> GenerateAccessToken(string? userId, DateTime expirationDate, ClaimsIdentity claimsIdentity,
        BaseAccessSignatureEntitiy? baseUserEntitiy);

    Task<bool> ValidateAccessTokenLifeTime(string? userToken);
}