namespace Auth.Infrastructure.DigitalSignature.DigitalSignatureOptions;

public sealed class AccessSignatureOptions
{
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}