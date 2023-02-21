using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory.InvestigationDisclosure;
using MongoDB.Driver;

namespace Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;

public interface IUserInvestigation
{
    Task<InvestigationResult> InvestigateUserInsertAsync(UserModel user, int limit, CancellationToken cancellationToken = default);
    Task<InvestigationResult> InvestigateUserUpdateAsync(UserModel user, UpdateDefinition<UserModel> updateDefinition, int limit, CancellationToken cancellationToken = default);

    Task<UserModel> FindOneAsync(UserModel user, int limit, CancellationToken cancellationToken = default);






}