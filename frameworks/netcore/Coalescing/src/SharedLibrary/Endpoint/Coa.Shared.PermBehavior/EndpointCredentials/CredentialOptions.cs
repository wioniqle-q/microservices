namespace Coa.Shared.PermBehavior.EndpointCredentials;

public sealed class CredentialOptions
{
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}