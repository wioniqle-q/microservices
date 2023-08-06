using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.Conclusions;

namespace Auth.Infrastructure.TransferProtocol.TransferInterfaces;

public interface ITransfer
{
    Task<TransferOutcomeValue> ValidateTransferRefreshToken(string token, BaseUserEntitiy baseUserEntitiy,
        BaseDevice baseDevice, CancellationToken cancellationToken = default);

    Task<TransferOutcomeValue> ValidateTransferAccessToken(string token, BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default);
}