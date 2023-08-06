using Auth.Domain.Entities.MongoEntities;

namespace Auth.Infrastructure.TransferProtocol.TransferInterfaces;

public interface IQuerySection
{
    Task<bool> CheckReuseToken(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    Task<bool> AddUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    Task<bool> CheckRefreshTokensCount(BaseUserEntitiy baseUserEntitiy, CancellationToken cancellationToken = default);
}