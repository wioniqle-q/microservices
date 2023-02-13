using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory.InvestigationDisclosure;

namespace Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;

public interface IUserInvestigation
{
    Task<InvestigationResult> InvestigateUserInsertAsync(UserModel user, int limit, CancellationToken cancellationToken = default);
}