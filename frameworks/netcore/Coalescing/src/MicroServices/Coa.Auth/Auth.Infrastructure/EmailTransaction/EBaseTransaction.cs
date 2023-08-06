using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Auth.Domain.Entities.MongoEntities;
using Auth.Domain.Entities.SignatureEntities;
using Auth.Infrastructure.AdlemanProtocol.AdlemanInterfaces;
using Auth.Infrastructure.ConcealmentProtocol.ConcealmentInterfaces;
using Auth.Infrastructure.DigitalSignature.DigitalSignatureInterfaces;
using Auth.Infrastructure.EmailTransaction.EmailTransactionAbstracts;
using Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;
using MongoDB.Driver;

namespace Auth.Infrastructure.EmailTransaction;

public sealed class EBaseTransaction : EmailTransactionAbstract
{
    private readonly IAdlemanIdentity _adleman;
    private readonly IConcealment _concealment;
    private readonly EBaseOptions _eBaseOptions;
    private readonly IUserHelper _userHelper;
    private readonly IUserSignature _userSignature;

    public EBaseTransaction(IUserSignature userSignature, IUserHelper userHelper, IAdlemanIdentity adlemanIdentity,
        EBaseOptions eBaseOptions, IConcealment concealment)
    {
        _userSignature = userSignature;
        _userHelper = userHelper;
        _adleman = adlemanIdentity;
        _eBaseOptions = eBaseOptions;
        _concealment = concealment;
    }

    public override async Task<string> VerifyEmailAsync(string userName, string signature, string publicKey)
    {
        var signatureValid = await _userSignature.ValidateUserTokenLifeTime(signature).ConfigureAwait(false);
        if (signatureValid is not true) return "Signature is not valid or expired";

        var nameFilter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, userName);

        var userQuery = await _userHelper.FindUserByQueryAsync(nameFilter);
        if (userQuery is null) return "User does not exist";

        if (userQuery.UserProperty.IsLocked is false && userQuery.UserProperty.IsEmailConfirmed is true &&
            userQuery.UserProperty.Require2Fa is false)
            return "User is already verified";

        var userRsa = await _concealment.RevealAsync(userQuery.UserRsa.RsaValidateKey, null, null);
        var privateKey = await _concealment.RevealAsync(userQuery.UserRsa.RsaPrivateKey, null, null);

        var signRsa = await _adleman.SignAsync(userRsa, privateKey);
        if (string.IsNullOrEmpty(signRsa)) return "Sign is not valid";

        var verifySign = await _adleman.VerifyAsync(userRsa, signRsa, publicKey);
        if (verifySign is not true) return "User information is not valid";

        var signatureEquality = await _userSignature.VerifyUserIdEqualAsync(signature, string.Concat(userQuery.UserId))
            .ConfigureAwait(false);
        if (signatureEquality is not true)
            return "Signature is not valid, you are trying to verify another user email";

        var replayAttack = await _userSignature.VerifyUserReplayToken(signature, DateTime.UtcNow).ConfigureAwait(false);
        if (replayAttack is true)
            return "You are not authorized to perform this action, please contact admin";

        var currentTime = DateTime.UtcNow;
        var updatedTime = currentTime.AddMinutes(10);
        var randomTransactionString = await GenerateRandomHexString().ConfigureAwait(false);

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

        var refreshToken = await _userSignature
            .GenerateUserRefreshToken(string.Concat(userQuery.UserId), userSignatureInfo.TrialDate, userSignatureInfo)
            .ConfigureAwait(false);
        if (string.IsNullOrEmpty(refreshToken))
            return "Operation failed, please contact admin (Email verification - R)";

        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userQuery.UserProperty.TimeZone);
        var userLocalTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, userTimeZone);
        var userUtcTime = userLocalTime.ToUniversalTime();

        var userFilter = Builders<BaseUserEntitiy>.Filter.Eq(x => x.UserName, userName);
        var userUpdate = Builders<BaseUserEntitiy>.Update
            .Set(x => x.UserProperty.IsEmailConfirmed, true)
            .Set(x => x.UserProperty.Require2Fa, false)
            .Set(x => x.UserProperty.IsLocked, false)
            .Set(x => x.UserProperty.LastLogin, string.Concat(userUtcTime))
            .Set(x => x.UserProperty.RefreshToken, refreshToken);

        var operation = await _userHelper.UpdateUserVerifyEmailAsync(userFilter, userUpdate, CancellationToken.None)
            .ConfigureAwait(false);

        return operation is not true
            ? "Operation failed, please contact admin (Email verification)"
            : "Email verified successfully";
    }

    public override async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_eBaseOptions.SmtpServer, _eBaseOptions.SmtpPort);
        smtpClient.EnableSsl = _eBaseOptions.EnableSsl;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.UseDefaultCredentials = false;
        if (smtpClient.UseDefaultCredentials is false)
            smtpClient.Credentials = new NetworkCredential(_eBaseOptions.FromMail, _eBaseOptions.FromMailPassword);

        smtpClient.Send(_eBaseOptions.FromMail, email, subject, message);

        await Task.CompletedTask;
    }

    private static async ValueTask<string> GenerateRandomHexString(int length = 36)
    {
        var randomBytes = new byte[length / 2];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return await new ValueTask<string>(BitConverter.ToString(randomBytes).Replace("-", string.Empty).ToLower());
    }
}