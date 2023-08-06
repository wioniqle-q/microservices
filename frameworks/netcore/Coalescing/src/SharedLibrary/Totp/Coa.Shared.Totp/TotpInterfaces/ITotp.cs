namespace Coa.Shared.Totp.TotpInterfaces;

public interface ITotp
{
    Task<string> GenerateTotpAsync(string secret, double digits);
    Task<bool> ValidateTotpAsync(string secret, string totp, double digits);
    Task<TimeSpan> GetRemainingTimeAsync();
    Task<TimeSpan> GetValidityDurationAsync();
}