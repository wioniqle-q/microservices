using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Coa.Shared.PermBehavior.EndpointCredentials.CredentialsAbstractions;
using Coa.Shared.PermBehavior.EndpointOutcomes.ConclusionsValue;
using Microsoft.IdentityModel.Tokens;

namespace Coa.Shared.PermBehavior.EndpointCredentials;

public sealed class EndpointCredential : EndpointCredentialAbstract
{
    private readonly CredentialOptions _credentialOptions;

    public EndpointCredential(CredentialOptions credentialOptions)
    {
        _credentialOptions = credentialOptions;
    }

    public override async Task<ValueConclusion> GenerateEndpointToken(DateTime expirationDate,
        ClaimsIdentity? claimsIdentity)
    {
        if (claimsIdentity is null || !claimsIdentity.Claims.Any() || expirationDate == default)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-GET): Argument Null Exception.",
                ExceptionMessage = "(EC-GET): ClaimsIdentity is null."
            };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_credentialOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = expirationDate,
            Issuer = _credentialOptions.Issuer,
            Audience = _credentialOptions.Audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        if (token is null)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-GE): Token Null Exception.",
                ExceptionMessage = "(EC-GE): Token is null."
            };

        return await Task.FromResult(new ValueConclusion
        {
            UniqueStatusCode = 2,
            Description = "(EC-GE): Endpoint token was generated successfully.",
            Token = tokenHandler.WriteToken(token) ?? string.Empty
        });
    }

    public override async Task<ValueConclusion> GenerateEndpointRefreshToken(DateTime expirationDate,
        ClaimsIdentity? claimsIdentity)
    {
        if (claimsIdentity is null || !claimsIdentity.Claims.Any() || expirationDate == default)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-GET): Argument Null Exception.",
                ExceptionMessage = "(EC-GET): ClaimsIdentity is null."
            };

        var result = await GenerateEndpointToken(expirationDate, claimsIdentity);
        if (result.UniqueStatusCode is 1)
            return result;

        return await Task.FromResult(new ValueConclusion
        {
            UniqueStatusCode = 2,
            Description = "(EC-GE): Endpoint refresh token was generated successfully.",
            Token = result.Token
        });
    }

    public override async Task<ValueConclusion> ValidateEndpointTokenLifeTime(string? endpointToken)
    {
        if (string.IsNullOrWhiteSpace(endpointToken))
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VETL): Argument Null or Empty Exception.",
                ExceptionMessage = "(EC-VETL): One or more parameters are null or empty."
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(endpointToken, validationParameters, out var securityToken);
            if (principal is null || securityToken is null)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VETL): Token Validation Exception.",
                    ExceptionMessage = "(EC-VETL): Token validation failed."
                };

            return new ValueConclusion
            {
                UniqueStatusCode = 2,
                Description = "(EC-VETL): Token validation was successful."
            };
        }
        catch
        {
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VETL): Token Validation Exception.",
                ExceptionMessage = "(EC-VETL): Token validation failed."
            };
        }
    }

    public override async Task<ValueConclusion> VerifyEndpointReplayToken(string? endpointToken, DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(endpointToken) || dateTime == default)
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VERT): Argument Null or Empty Exception.",
                ExceptionMessage = "(EC-VERT): One or more parameters are null or empty."
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(endpointToken, validationParameters, out var securityToken);
            if (principal is null || securityToken is null)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VERT): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VERT): Token Validation Exception.",
                    ExceptionMessage = "(EC-VERT): Token validation failed."
                };

            if (securityToken is not JwtSecurityToken token)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VERT): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VERT): Token Null Exception.",
                    ExceptionMessage = "(EC-VERT): Token is null."
                };

            var tokenDate = token.Claims.FirstOrDefault(c => c.Type == "CreatedDate")?.Value;
            if (tokenDate == default)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VERT): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VERT): Token Date Null Exception.",
                    ExceptionMessage = "(EC-VERT): Token date is null."
                };

            var tokenDateUtc = DateTime.Parse(tokenDate).ToUniversalTime();
            var dateTimeUtc = dateTime.ToUniversalTime();

            if (tokenDateUtc >= dateTimeUtc)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VERT): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VERT): Token Replay Attack Exception.",
                    ExceptionMessage = "(EC-VERT): Token is a replay attack."
                };

            return new ValueConclusion
            {
                UniqueStatusCode = 2,
                Description = "(EC-VERT): Token validation was successful."
            };
        }
        catch
        {
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "(EC-VERT): Token Validation Exception.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VERT): Token Validation Exception.",
                ExceptionMessage = "(EC-VERT): Token validation failed."
            };
        }
    }

    public override async Task<ValueConclusion> VerifyEndpointIdEqualAsync(string? endpointToken, string? endpointId)
    {
        if (string.IsNullOrWhiteSpace(endpointToken) || string.IsNullOrWhiteSpace(endpointId))
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VIE): Argument Null or Empty Exception.",
                ExceptionMessage = "(EC-VIE): One or more parameters are null or empty."
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(endpointToken, validationParameters, out var securityToken);
            if (principal is null || securityToken is null)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VIE): Token Validation Exception.",
                    ExceptionMessage = "(EC-VIE): Token validation failed."
                };

            if (securityToken is not JwtSecurityToken token)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VIE): Token Null Exception.",
                    ExceptionMessage = "(EC-VIE): Token is null."
                };

            var tokenEndpointId = token.Claims.FirstOrDefault(c => c.Type == "EndpointId")?.Value;
            if (string.IsNullOrWhiteSpace(tokenEndpointId))
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VIE): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VIE): Token EndpointId Null Exception.",
                    ExceptionMessage = "(EC-VIE): Token endpointId is null."
                };

            if (tokenEndpointId != endpointId)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VIE): Token Validation Exception. Please check your specified values.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VIE): Token EndpointId Exception.",
                    ExceptionMessage = "(EC-VIE): Token endpointId is not equal to endpointId."
                };

            return new ValueConclusion
            {
                UniqueStatusCode = 2,
                Description = "(EC-VIE): Token validation was successful."
            };
        }
        catch
        {
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VIE): Token Validation Exception.",
                ExceptionMessage = "(EC-VIE): Token validation failed."
            };
        }
    }

    public override async Task<ValueConclusion> VerifyEndpointAccessBehavior(string? endpointToken,
        ClaimsIdentity? claimsIdentity)
    {
        if (string.IsNullOrWhiteSpace(endpointToken) || claimsIdentity is null || !claimsIdentity.Claims.Any())
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "(EC-VEAB): Argument Null or Empty Exception.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VEAB): Argument Null or Empty Exception.",
                ExceptionMessage = "(EC-VEAB): One or more parameters are null or empty."
            };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();

        try
        {
            var principal = tokenHandler.ValidateToken(endpointToken, validationParameters, out var securityToken);
            if (principal is null || securityToken is null)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VEAB): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VEAB): Token Validation Exception.",
                    ExceptionMessage = "(EC-VEAB): Token validation failed."
                };

            if (securityToken is not JwtSecurityToken token)
                return new ValueConclusion
                {
                    UniqueStatusCode = 1,
                    Description = "(EC-VEAB): Token Validation Exception.",
                    ExceptionId = Guid.NewGuid().ToString(),
                    ExceptionType = "(EC-VEAB): Token Null Exception.",
                    ExceptionMessage = "(EC-VEAB): Token is null."
                };

            var nonMatchingClaims = (from claim in claimsIdentity.Claims
                let matchingClaim = token.Claims.FirstOrDefault(c =>
                    c.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase)
                    && c.Value.Equals(claim.Value, StringComparison.Ordinal))
                where matchingClaim == null
                select claim).ToList();

            if (nonMatchingClaims.Count is 0)
                return new ValueConclusion
                {
                    UniqueStatusCode = 2,
                    Description = "(EC-VEAB): Authorized to access this endpoint."
                };

            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "(EC-VEAB): Token Claims Mismatch Exception.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VEAB): Token Claims Mismatch Exception.",
                ExceptionMessage = "(EC-VEAB): Token claims do not match the provided claimsIdentity."
            };
        }
        catch
        {
            return new ValueConclusion
            {
                UniqueStatusCode = 1,
                Description = "(EC-VEAB): Token Validation Exception.",
                ExceptionId = Guid.NewGuid().ToString(),
                ExceptionType = "(EC-VEAB): Token Validation Exception.",
                ExceptionMessage = "(EC-VEAB): Token validation failed."
            };
        }
    }

    private async Task<TokenValidationParameters> GetValidationParameters()
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_credentialOptions.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _credentialOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _credentialOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        return await Task.FromResult(validationParameters);
    }
}