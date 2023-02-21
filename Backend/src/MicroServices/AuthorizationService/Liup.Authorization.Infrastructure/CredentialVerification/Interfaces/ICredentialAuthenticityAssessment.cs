using Liup.Authorization.Domain.Models.MongoModels;
using Liup.Authorization.Infrastructure.CredentialVerification.MongoDB.CVerification.CResults;

namespace Liup.Authorization.Infrastructure.CredentialVerification.Interfaces;
public interface ICredentialAuthenticityAssessment
{
    Task<CredentialResult> AssessCredentialAuthenticityAsync(UserModelAssesment credential, CancellationToken cancellationToken);
}
