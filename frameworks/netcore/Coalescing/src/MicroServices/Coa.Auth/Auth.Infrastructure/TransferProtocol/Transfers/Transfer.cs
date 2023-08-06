using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.TransferProtocol.Conclusions;
using Auth.Infrastructure.TransferProtocol.TransferAbstractions;
using Auth.Infrastructure.TransferProtocol.TransferConfigurations;
using Auth.Infrastructure.TransferProtocol.TransferInterfaces;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using ConfigCat.Client;

namespace Auth.Infrastructure.TransferProtocol.Transfers;

public sealed class Transfer : TransferAbstract
{
    private readonly IAccessSignature _accessSignature;
    private readonly IArtifactSection _artifactSection;
    private readonly CatConfiguration _catConfiguration;
    private readonly IQuerySection _querySection;
    private readonly IUserHelper _userHelper;
    private readonly IUserSignature _userSignature;

    public Transfer(
        IAccessSignature accessSignature,
        IArtifactSection artifactSection,
        IQuerySection querySection,
        IUserHelper userHelper,
        IUserSignature userSignature,
        CatConfiguration catConfiguration
    )
    {
        _accessSignature = accessSignature;
        _artifactSection = artifactSection;
        _querySection = querySection;
        _userHelper = userHelper;
        _userSignature = userSignature;
        _catConfiguration = catConfiguration;
    }

    public override async Task<TransferOutcomeValue> ValidateTransferRefreshToken(string token,
        BaseUserEntitiy? baseUserEntitiy, BaseDevice? baseDevice,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || baseUserEntitiy is null || baseDevice is null)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Incorrect credentials, please fill in all fields"
            };

        var tokenNameIdentifier = await _userSignature.GetTokenNameIdentifier(token).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(tokenNameIdentifier))
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "The name identifier is null or empty, please contact the administrator"
            };

        var userId = ConvertStringToGuid(tokenNameIdentifier);
        if (userId == Guid.Empty)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "User ID is blank"
            };

        var user = await _userHelper.GetUserByIdAsync(userId, CancellationToken.None).ConfigureAwait(false);
        if (user is null)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "User not found"
            };

        var tokenLifeTime = await _userSignature.ValidateUserTokenLifeTime(token).ConfigureAwait(false);
        if (tokenLifeTime is false)
        {
            var validatedInformation = await ValidateAllInformation(token, user, baseDevice, CancellationToken.None)
                .ConfigureAwait(false);
            return new TransferOutcomeValue
            {
                Status = validatedInformation.Status,
                Description = validatedInformation.Description
            };
        }

        var validateAll = await ValidateAllInformation(token, user, baseDevice, CancellationToken.None)
            .ConfigureAwait(false);
        return new TransferOutcomeValue
        {
            Status = validateAll.Status,
            Description = validateAll.Description
        };
    }

    public override async Task<TransferOutcomeValue> ValidateTransferAccessToken(string token,
        BaseUserEntitiy baseUserEntitiy,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<TransferOutcomeValue> ValidateAllInformation(string token,
        BaseUserEntitiy baseUserEntitiy, BaseDevice baseDevice,
        CancellationToken cancellationToken = default)
    {
        var firstFlag = _catConfiguration.Flags.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstFlag))
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "First flag is null or empty"
            };

        var client = ConfigCatClient.Get(_catConfiguration.SdkKey);
        var isCompareDevices = await client.GetValueAsync(firstFlag, false, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        client.Dispose();

        var compareDevices = await _artifactSection
            .CheckDeviceInfo<bool>(baseUserEntitiy, baseDevice, CancellationToken.None)
            .ConfigureAwait(false);
        if (compareDevices is false && isCompareDevices)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Device info not matched"
            };

        var reuseToken = await _querySection.CheckReuseToken(baseUserEntitiy, token, CancellationToken.None)
            .ConfigureAwait(false);
        if (reuseToken is true)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Token already used"
            };

        var removedRefreshToken = await _querySection
            .RemoveUserRefreshTokenAsync(baseUserEntitiy, token, CancellationToken.None)
            .ConfigureAwait(false);
        if (removedRefreshToken is false)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Refresh token has not been removed"
            };

        var randomTransactionString = await _artifactSection.GenerateRandomHexString(36, CancellationToken.None)
            .ConfigureAwait(false);
        var currentTime = DateTime.UtcNow;
        var updatedTime = currentTime.AddMinutes(10);

        var userSignatureInfo = new BaseUserSignatureEntitiy
        {
            TransactionId = randomTransactionString,
            TrialStatus = true,
            IsAuthorized = false,
            CustomAuthorization = "TRIAL-USER",
            IsBlocked = false,
            OccurrenceTime = currentTime.ToString("O"),
            EnrollmentDate = updatedTime.ToString("O"),
            TrialDate = updatedTime
        };

        var newRefreshToken = await _userSignature
            .GenerateUserRefreshToken(string.Concat(baseUserEntitiy.UserId), userSignatureInfo.TrialDate,
                userSignatureInfo).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(newRefreshToken))
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Failed to generate new refresh token"
            };

        var addRefreshToken = await _querySection
            .AddUserRefreshTokenAsync(baseUserEntitiy, newRefreshToken, CancellationToken.None)
            .ConfigureAwait(false);
        if (addRefreshToken is false)
            return new TransferOutcomeValue
            {
                Status = false,
                Description = "Token not added to update tokens list"
            };

        return new TransferOutcomeValue
        {
            Status = true,
            Description = newRefreshToken
        };
    }

    private static Guid ConvertStringToGuid(string guidString)
    {
        if (string.IsNullOrEmpty(guidString))
            return Guid.Empty;

        return !Guid.TryParse(guidString, out var result) ? Guid.Empty : result;
    }
}