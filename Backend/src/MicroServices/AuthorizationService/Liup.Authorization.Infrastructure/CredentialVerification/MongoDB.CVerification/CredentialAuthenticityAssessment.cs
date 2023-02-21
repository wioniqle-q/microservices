using Liup.Authorization.Domain.Models.MongoModels;
using Liup.Authorization.Infrastructure.CredentialVerification.Interfaces;
using Liup.Authorization.Infrastructure.CredentialVerification.MongoDB.CVerification.CResults;
using Liup.Authorization.Infrastructure.GlobalEncryption.Interfaces;
using Liup.UserInteraction.Domain.Models.MongoModels;
using Liup.UserInteraction.Infrastructure.PasswordSecurity;
using Liup.UserInteraction.Infrastructure.UserDirectory.MongoDirectory;

namespace Liup.Authorization.Infrastructure.CredentialVerification.MongoDB.CVerification;

public class CredentialAuthenticityAssessment : ICredentialAuthenticityAssessment
{
    private readonly IUserInvestigation _userInvestigation;
    private readonly IEncryptionProvider _encryptionProvider;

    public CredentialAuthenticityAssessment(IUserInvestigation userInvestigation, IEncryptionProvider encryptionProvider)
    {
        _userInvestigation = userInvestigation;
        _encryptionProvider = encryptionProvider;
    }

    public async virtual Task<CredentialResult> AssessCredentialAuthenticityAsync(UserModelAssesment credential, CancellationToken cancellationToken)
    {
        var paramsResult = await VerifyCredentialPasswordAsync(credential, cancellationToken).ConfigureAwait(false);
        if (paramsResult is null)
        {
            return await Task.FromCanceled<CredentialResult>(cancellationToken);
        }

        return await new ValueTask<CredentialResult>(paramsResult);
    }

    protected async virtual ValueTask<CredentialResult> VerifyCredentialPasswordAsync(UserModelAssesment credential, CancellationToken cancellationToken)
    {
        var CredentialInformation = await GetCredentialInformationAsync(credential, cancellationToken).ConfigureAwait(false);
        if (CredentialInformation.UserName == null || CredentialInformation.Password == null)
        {
            return await Task.FromCanceled<CredentialResult>(cancellationToken);
        }

        Console.WriteLine(CredentialInformation.UserName);

        var userExists = await _userInvestigation.FindOneAsync(new UserModel() { UserName = CredentialInformation.UserName }, 1, cancellationToken).ConfigureAwait(false);
        if (userExists is null || userExists.UserProperties.IsLocked is true)
        {
            return await new ValueTask<CredentialResult>(new CredentialResult("User could not be found"));
        }

        var passwordVerified = await VerifyPasswordAsync(CredentialInformation.Password, userExists, cancellationToken).ConfigureAwait(false);
        return await new ValueTask<CredentialResult>(passwordVerified);
    }

    protected async virtual ValueTask<CredentialResult> VerifyPasswordAsync(string password, UserModel credential, CancellationToken cancellationToken)
    {
        if (password is null || credential.Password is null)
        {
            return await Task.FromCanceled<CredentialResult>(cancellationToken);
        }

        var passwordVerified = await PasswordSecurity.VerifyPassword(password, credential.Password).ConfigureAwait(false);
        if (passwordVerified is not true)
        {
            return await new ValueTask<CredentialResult>(new CredentialResult("Incorrect password"));
        }

        return await new ValueTask<CredentialResult>(new CredentialResult("Password verified"));
    }

    protected async virtual ValueTask<UserModelAssesment> GetCredentialInformationAsync(UserModelAssesment? credential, CancellationToken cancellationToken)
    {
        if (credential?.UserName is null || credential?.Password is null)
        {
            return await Task.FromCanceled<UserModelAssesment>(cancellationToken);
        }

        var x = await _encryptionProvider.EncryptAsync(credential.UserName, null, null, cancellationToken);
        var y = await _encryptionProvider.EncryptAsync(credential.Password, null, null, cancellationToken);

        Console.WriteLine(x);
        Console.WriteLine("\n");
        Console.WriteLine(y);

        credential.UserName = x;
        credential.Password = y;

        var CredentialInformation = new UserModelAssesment()
        {
            UserName = await _encryptionProvider.DecryptAsync(credential.UserName, null, null, cancellationToken),
            Password = await _encryptionProvider.DecryptAsync(credential.Password, null, null, cancellationToken)
        };

        return await new ValueTask<UserModelAssesment>(CredentialInformation);
    }
}