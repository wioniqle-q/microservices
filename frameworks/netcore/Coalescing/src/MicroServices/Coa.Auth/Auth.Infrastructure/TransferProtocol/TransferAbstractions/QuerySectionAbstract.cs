using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;

namespace Auth.Infrastructure.TransferProtocol.TransferAbstractions;

public abstract class QuerySectionAbstract : IQuerySection
{
    public abstract Task<bool> CheckReuseToken(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> RemoveUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> AddUserRefreshTokenAsync(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> CheckRefreshTokensCount(BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default);
}