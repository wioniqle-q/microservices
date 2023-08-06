using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;

namespace Auth.Infrastructure.UserTransaction.Interfaces;

public interface ICredentialAssesment
{
    Task<OutcomeValue> AssessAsync(BaseUserEntitiy user);
}