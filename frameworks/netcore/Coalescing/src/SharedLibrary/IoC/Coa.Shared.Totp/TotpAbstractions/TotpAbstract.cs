using Coa.Shared.Totp.TotpInterfaces;

namespace Coa.Shared.Totp.TotpAbstractions;

public abstract class TotpAbstract : ITotp
{
    public abstract Task<string> GenerateTotpAsync(string secret, double digits);
    public abstract Task<bool> ValidateTotpAsync(string secret, string totp, double digits);
    public abstract Task<TimeSpan> GetRemainingTimeAsync();
    public abstract Task<TimeSpan> GetValidityDurationAsync();
}