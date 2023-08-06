using System.Net.Http.Headers;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;
using Coa.Shared.PermBehavior.EndpointValidators.ValidatorsAbstractions;

namespace Coa.Shared.PermBehavior.EndpointValidators;

public sealed class EndpointValidate : EndpointValidateAbstract
{
    public override async Task<ValueConclusion> ValidateAccessBehavior(HttpRequestMessage httpContent)
    {
        var authorizationHeaderValidationResult = await ValidateAuthorizationHeader(httpContent).ConfigureAwait(false);
        if (authorizationHeaderValidationResult.UniqueStatusCode is 2)
            return authorizationHeaderValidationResult;

        var accessTokenValidationResult = await ValidateAccessToken(httpContent).ConfigureAwait(false);
        return accessTokenValidationResult.UniqueStatusCode is 2
            ? accessTokenValidationResult
            : await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "No access is present.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "No access is present.",
                ExceptionMessage = "No access is present."
            });
    }

    private static async Task<ValueConclusion> ValidateAuthorizationHeader(HttpRequestMessage httpContent)
    {
        if (!httpContent.Headers.TryGetValues("Authorization", out var authorizationHeaders))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Authorization header not found or token not found in headers.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Authorization header not found or token not found in header.",
                ExceptionMessage = "Authorization header not found or token not found in header."
            });

        var authorizationHeader = authorizationHeaders.FirstOrDefault();

        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var header))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Authorization header not found or token not found in header.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Authorization header not found or token not found in header.",
                ExceptionMessage = "Authorization header not found or token not found in header."
            });

        if (!header.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Authorization header not found or token not found in header.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Authorization header not found or token not found in header.",
                ExceptionMessage = "Authorization header not found or token not found in header."
            });

        var token = header.Parameter?.Trim();
        if (string.IsNullOrWhiteSpace(token))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Authorization header not found or token not found in header.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Authorization header not found or token not found in header.",
                ExceptionMessage = "Authorization header not found or token not found in header."
            });

        return await Task.FromResult(new ValueConclusion
        {
            UniqueStatusCode = 2,
            Description = "Valid authorization header.",
            Token = token
        });
    }

    private static async Task<ValueConclusion> ValidateAccessToken(HttpRequestMessage httpContent)
    {
        if (!httpContent.Headers.TryGetValues("X-HTTP-Method-Override", out var httpMethod) ||
            !string.Equals(httpMethod.FirstOrDefault(), "POST", StringComparison.OrdinalIgnoreCase))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Invalid request method.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Invalid request method.",
                ExceptionMessage = "Invalid request method."
            });

        var accessToken = httpContent.Headers.TryGetValues("access_token", out var accessTokenHeader)
            ? string.Concat(accessTokenHeader)
            : string.Empty;

        if (string.IsNullOrWhiteSpace(accessToken))
            return await Task.FromResult(new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Invalid access token.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "Invalid access token.",
                ExceptionMessage = "Invalid access token."
            });

        return await Task.FromResult(string.IsNullOrWhiteSpace(accessToken)
            ? new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "Invalid access token."
            }
            : new ValueConclusion
            {
                UniqueStatusCode = 2,
                Token = accessToken
            });
    }
}