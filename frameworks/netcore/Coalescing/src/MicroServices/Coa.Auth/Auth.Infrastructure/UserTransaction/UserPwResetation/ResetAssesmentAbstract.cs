using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;
using Auth.Infrastructure.UserTransaction.Interfaces;

namespace Auth.Infrastructure.UserTransaction.UserPwResetation;

public abstract class ResetAssesmentAbstract : IResetAssesment
{
    public abstract Task<OutcomeValue> ResetAssesmentAsync(BaseUserEntitiy? user, string userNewPassword,
        CancellationToken cancellationToken = default);
}