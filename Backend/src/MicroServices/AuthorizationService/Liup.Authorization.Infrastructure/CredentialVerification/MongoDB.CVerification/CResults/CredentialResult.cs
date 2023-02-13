
namespace Liup.Authorization.Infrastructure.CredentialVerification.MongoDB.CVerification.CResults;

public abstract class CredentialAssesmentResult
{
    public virtual string? CredentialResponse { get; set; }
}

public sealed class CredentialResult : CredentialAssesmentResult
{
    public CredentialResult(string credentialResponse)
    {
        CredentialResponse = credentialResponse;
    }

    public sealed override string? CredentialResponse
    {
        get => credentialResponse;
        set => credentialResponse = value;
    }
    public volatile string? credentialResponse;
}
