namespace Auth.Infrastructure.TransferProtocol.TransferConfigurations;

public sealed class CatConfiguration
{
    public string SdkKey { get; set; } = string.Empty;
    public string[] Flags { get; set; } = Array.Empty<string>();
}