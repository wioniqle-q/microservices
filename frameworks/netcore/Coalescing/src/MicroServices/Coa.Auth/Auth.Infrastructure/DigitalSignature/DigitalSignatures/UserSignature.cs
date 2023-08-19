using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureAbstracts;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureOptions;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.DigitalSignature.DigitalSignatures;

public sealed class UserSignature : UserSignatureAbstract
{
    private readonly UserSignatureOptions _userSignatureOptions;

    public UserSignature(UserSignatureOptions userSignatureOptions)
    {
        _userSignatureOptions = userSignatureOptions;
    }

    public override async Task<string> GenerateUserToken(string? userId, DateTime expirationDate,
        BaseUserSignatureEntitiy? baseUserEntitiy)
    {
        if (string.IsNullOrEmpty(userId) || expirationDate == default || baseUserEntitiy == null)
            throw new ArgumentException("Invalid arguments U-S");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userSignatureOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("TransactionId", baseUserEntitiy.TransactionId),
                new Claim("TrialStatus", baseUserEntitiy.TrialStatus.ToString()),
                new Claim("IsAuthorized", baseUserEntitiy.IsAuthorized.ToString()),
                new Claim("CustomAuthorization", baseUserEntitiy.CustomAuthorization),
                new Claim("IsBlocked", baseUserEntitiy.IsBlocked.ToString()),
                new Claim("occurrenceTime", baseUserEntitiy.OccurrenceTime),
                new Claim("enrollmentDate", baseUserEntitiy.EnrollmentDate)
            }),
            Expires = expirationDate,
            Issuer = _userSignatureOptions.Issuer,
            Audience = _userSignatureOptions.Audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return await Task.FromResult(tokenHandler.WriteToken(token));
    }

    public override async Task<string> GenerateUserRefreshToken(string? userId, DateTime expirationDate,
        BaseUserSignatureEntitiy? baseUserEntitiy)
    {
        if (string.IsNullOrEmpty(userId) || expirationDate == default || baseUserEntitiy == null)
            throw new ArgumentException("Invalid arguments U-S");

        var generateUserToken =
            await GenerateUserToken(userId, expirationDate, baseUserEntitiy);
        if (string.IsNullOrEmpty(generateUserToken))
            throw new ArgumentException("Invalid arguments U-G-S");

        return await Task.FromResult(generateUserToken);
    }

    public override async Task<string> GetTokenNameIdentifier(string? token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Invalid arguments T (U-S)");

        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(token))
            throw new ArgumentException("Invalid JWT token S (U-S)", nameof(token));

        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        var nameIdentifier = securityToken?.Claims?.FirstOrDefault(claim => claim.Type == "nameid")?.Value;
        if (string.IsNullOrEmpty(nameIdentifier))
            throw new ArgumentException("Invalid JWT token C (U-S)", nameof(token));

        return await Task.FromResult(nameIdentifier);
    }

    public override async Task<bool> ValidateUserTokenLifeTime(string? userToken)
    {
        if (string.IsNullOrEmpty(userToken))
            throw new ArgumentException("Invalid arguments U-T (U-S)");

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();

        try
        {
            tokenHandler.ValidateToken(userToken, validationParameters, out _);
            return await Task.FromResult(true);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    public override async Task<bool> VerifyUserIdEqualAsync(string? userToken, string? userId)
    {
        if (string.IsNullOrEmpty(userToken) || string.IsNullOrEmpty(userId))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(userToken))
            return false;

        var validationParameters = await GetValidationParameters();

        try
        {
            tokenHandler.ValidateToken(userToken, validationParameters, out _);

            var nameIdentifier = await GetTokenNameIdentifier(userToken);
            if (string.IsNullOrEmpty(nameIdentifier))
                return false;

            return await Task.FromResult(nameIdentifier == userId);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    public override async Task<bool> VerifyUserReplayToken(string? userToken, DateTime dateTime)
    {
        if (string.IsNullOrEmpty(userToken) || dateTime == default)
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(userToken))
            return false;

        var validationParameters = await GetValidationParameters();

        try
        {
            tokenHandler.ValidateToken(userToken, validationParameters, out _);

            if (tokenHandler.ReadToken(userToken) is not JwtSecurityToken securityToken)
                return false;

            var occurrenceTime = securityToken.Claims.FirstOrDefault(claim => claim.Type == "occurrenceTime")?.Value;
            if (string.IsNullOrEmpty(occurrenceTime))
                return false;

            var occurrenceTimeUtc = DateTime
                .ParseExact(occurrenceTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                .ToUniversalTime();
            var dateTimeUtc = dateTime.ToUniversalTime().AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            if (occurrenceTimeUtc >= dateTimeUtc)
                return true;

            return await Task.FromResult(false);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    private async Task<TokenValidationParameters> GetValidationParameters()
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userSignatureOptions.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _userSignatureOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _userSignatureOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        return await Task.FromResult(validationParameters);
    }
}