using System.Security.Claims;
using Coa.Shared.PermBehavior.EndpointAccessors.AccessorsAbstractions;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsInterfaces;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;

namespace Coa.Shared.PermBehavior.EndpointAccessors;

public sealed class EndpointAccessor : EndpointAccessorAbstract
{
    private readonly IEndpointCredential _endpointCredential;

    public EndpointAccessor(IEndpointCredential endpointCredential)
    {
        _endpointCredential = endpointCredential;
    }

    public override async Task<ValueConclusion> VerifyAccessBehavior(string endpointToken,
        ClaimsIdentity? claimsIdentity)
    {
        if (string.IsNullOrWhiteSpace(endpointToken) || endpointToken.StartsWith("Bearer "))
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(E-VA): Endpoint Permission - Verify Access Behavior.",
                ExceptionMessage = "(E-VA): Please provide of the access token."
            };

        if (claimsIdentity is null)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(E-VA): Endpoint Permission - Verify Access Behavior.",
                ExceptionMessage = "(E-VA): Please provide of the access values."
            };

        var tokenLifeTime =
            await _endpointCredential.ValidateEndpointTokenLifeTime(endpointToken);
        if (tokenLifeTime.UniqueStatusCode is 1)
            return tokenLifeTime;

        var tokenReplayAttack = await _endpointCredential.VerifyEndpointReplayToken(endpointToken, DateTime.UtcNow);
        if (tokenReplayAttack.UniqueStatusCode is 1)
            return tokenReplayAttack;

        var tokenIdEqual = await _endpointCredential.VerifyEndpointIdEqualAsync(endpointToken,
            claimsIdentity.Claims.FirstOrDefault(x => x.Type == "EndpointId")?.Value);
        if (tokenIdEqual.UniqueStatusCode is 1)
            return tokenIdEqual;

        var verifyAccessToken = await _endpointCredential.VerifyEndpointAccessBehavior(endpointToken, claimsIdentity)
            ;
        return verifyAccessToken;
    }
}