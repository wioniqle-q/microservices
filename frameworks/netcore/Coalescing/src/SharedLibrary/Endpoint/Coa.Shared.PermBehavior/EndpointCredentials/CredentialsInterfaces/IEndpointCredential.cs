using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointCredentials.CredentialsInterfaces;

public interface IEndpointCredential
{
    Task<ValueConclusion> GenerateEndpointToken(DateTime expirationDate, ClaimsIdentity? claimsIdentity);
    Task<ValueConclusion> GenerateEndpointRefreshToken(DateTime expirationDate, ClaimsIdentity? claimsIdentity);

    Task<ValueConclusion> ValidateEndpointTokenLifeTime(string? endpointToken);
    Task<ValueConclusion> VerifyEndpointReplayToken(string? endpointToken, DateTime dateTime);
    Task<ValueConclusion> VerifyEndpointIdEqualAsync(string? endpointToken, string? endpointId);

    Task<ValueConclusion> VerifyEndpointAccessBehavior(string? endpointToken, ClaimsIdentity? claimsIdentity);
}