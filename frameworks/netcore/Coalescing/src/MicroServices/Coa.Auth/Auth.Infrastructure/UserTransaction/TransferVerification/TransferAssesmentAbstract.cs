using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.TransferVerification;

public abstract class TransferAssesmentAbstract : ITransferAssesment
{
    public abstract Task<OutcomeValue> AssessTransferAsync(BaseUserEntitiy user);
}