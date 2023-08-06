using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.UserVerification;

public abstract class CredentialAssesmentAbstract : ICredentialAssesment
{
    public abstract Task<OutcomeValue> AssessAsync(BaseUserEntitiy user);
}