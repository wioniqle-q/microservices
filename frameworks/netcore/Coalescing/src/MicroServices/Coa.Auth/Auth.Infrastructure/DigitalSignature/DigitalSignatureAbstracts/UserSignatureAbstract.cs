using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatureAbstracts;

public abstract class UserSignatureAbstract : IUserSignature
{
    public abstract Task<string> GenerateUserToken(string? userId, DateTime expirationDate,
        BaseUserSignatureEntitiy baseUserEntitiy);

    public abstract Task<string> GenerateUserRefreshToken(string? userId, DateTime expirationDate,
        BaseUserSignatureEntitiy baseUserEntitiy);

    public abstract Task<string> GetTokenNameIdentifier(string? token);
    public abstract Task<bool> ValidateUserTokenLifeTime(string? userToken);
    public abstract Task<bool> VerifyUserIdEqualAsync(string? userToken, string? userId);
    public abstract Task<bool> VerifyUserReplayToken(string? userToken, DateTime dateTime);
}