using Auth.Domain.Entities.SignatureEntities;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;

public interface IUserSignature
{
    Task<string> GenerateUserToken(string? userId, DateTime expirationDate, BaseUserSignatureEntitiy baseUserEntitiy);

    Task<string> GenerateUserRefreshToken(string? userId, DateTime expirationDate,
        BaseUserSignatureEntitiy baseUserEntitiy);

    Task<string> GetTokenNameIdentifier(string? token);
    Task<bool> ValidateUserTokenLifeTime(string? userToken);
    Task<bool> VerifyUserIdEqualAsync(string? userToken, string? userId);
    Task<bool> VerifyUserReplayToken(string? userToken, DateTime dateTime);
}