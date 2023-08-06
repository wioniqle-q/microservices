using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.UserTransaction.Conclusions;

namespace Auth.Infrastructure.UserTransaction.Interfaces;

public interface IResetAssesment
{
    Task<OutcomeValue> ResetAssesmentAsync(BaseUserEntitiy? user, string userNewPassword,
        CancellationToken cancellationToken = default);
}