using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;

namespace Auth.Infrastructure.UserTransaction.Interfaces;

public interface IInsertAssesment
{
    Task<OutcomeValue> InsertAssesmentAsync(BaseUserEntitiy user, CancellationToken cancellationToken = default);
}