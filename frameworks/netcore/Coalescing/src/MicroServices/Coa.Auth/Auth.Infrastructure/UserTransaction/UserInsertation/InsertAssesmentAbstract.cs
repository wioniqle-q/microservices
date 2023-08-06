using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.UserInsertation;

public abstract class InsertAssesmentAbstract : IInsertAssesment
{
    public abstract Task<OutcomeValue> InsertAssesmentAsync(BaseUserEntitiy user,
        CancellationToken cancellationToken = default);
}