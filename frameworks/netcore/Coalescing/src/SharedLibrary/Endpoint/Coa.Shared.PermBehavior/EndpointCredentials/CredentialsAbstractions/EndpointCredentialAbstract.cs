using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointCredentials.CredentialsAbstractions;

public abstract class EndpointCredentialAbstract : IEndpointCredential
{
    public abstract Task<ValueConclusion>
        GenerateEndpointToken(DateTime expirationDate, ClaimsIdentity? claimsIdentity);

    public abstract Task<ValueConclusion> GenerateEndpointRefreshToken(DateTime expirationDate,
        ClaimsIdentity? claimsIdentity);

    public abstract Task<ValueConclusion> ValidateEndpointTokenLifeTime(string? endpointToken);
    public abstract Task<ValueConclusion> VerifyEndpointReplayToken(string? endpointToken, DateTime dateTime);
    public abstract Task<ValueConclusion> VerifyEndpointIdEqualAsync(string? endpointToken, string? endpointId);

    public abstract Task<ValueConclusion> VerifyEndpointAccessBehavior(string? endpointToken,
        ClaimsIdentity? claimsIdentity);
}