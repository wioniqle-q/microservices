using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.Conclusions;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;

namespace Auth.Infrastructure.TransferProtocol.TransferAbstractions;

public abstract class TransferAbstract : ITransfer
{
    public abstract Task<TransferOutcomeValue> ValidateTransferRefreshToken(string token,
        BaseUserEntitiy baseUserEntitiy, BaseDevice baseDevice,
        CancellationToken cancellationToken = default);

    public abstract Task<TransferOutcomeValue> ValidateTransferAccessToken(string token,
        BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default);
}