using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureAbstracts;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureOptions;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatures;

public sealed class AccessSignature : AccessSignatureAbstract
{
    private readonly AccessSignatureOptions _accessSignatureOptions;

    public AccessSignature(AccessSignatureOptions accessSignatureOptions)
    {
        _accessSignatureOptions = accessSignatureOptions;
    }

    /*
     new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("TransactionId", baseAccessEntitiy.TransactionId),
                new Claim("TrialStatus", baseAccessEntitiy.TrialStatus.ToString()),
                new Claim("UserId", baseAccessEntitiy.UserId),
                new Claim("UserName", baseAccessEntitiy.UserName),
                new Claim("Email", baseAccessEntitiy.Email),
                new Claim("IsAccess", baseAccessEntitiy.IsAccess.ToString()),
                new Claim("CustomAuthorization", baseAccessEntitiy.CustomAuthorization),
                new Claim("occurrenceTime", baseAccessEntitiy.OccurrenceTime), // Access creation date
                new Claim("enrollmentDate", baseAccessEntitiy.EnrollmentDate) // Access expiration date
            }),
     */

    public override async Task<string> GenerateAccessToken(string? userId, DateTime expirationDate,
        ClaimsIdentity claimsIdentity,
        BaseAccessSignatureEntitiy? baseAccessEntitiy)
    {
        if (string.IsNullOrEmpty(userId) || expirationDate == default || baseAccessEntitiy == null)
            return await Task.FromResult(string.Empty);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessSignatureOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = expirationDate,
            Issuer = _accessSignatureOptions.Issuer,
            Audience = _accessSignatureOptions.Audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return await Task.FromResult(tokenHandler.WriteToken(token));
    }

    public override async Task<bool> ValidateAccessTokenLifeTime(string? userToken)
    {
        if (string.IsNullOrEmpty(userToken)) return await Task.FromResult(false);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessSignatureOptions.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _accessSignatureOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _accessSignatureOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(userToken, validationParameters, out _);
            return await Task.FromResult(true);
        }
        catch (Exception)
        {
            return await Task.FromResult(false);
        }
    }
}