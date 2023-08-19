using System.Security.Cryptography;
using System.Text;
using Coa.Shared.Totp.TotpAbstractions;

namespace Coa.Shared.Totp.TotpAccessors;

public sealed class TotpBase : TotpAbstract
{
    private const double DefaultPeriod = 3;
    private const double DefaultDigits = 8;

    private readonly Encoding _encoding = new UTF8Encoding(false, true);
    private readonly TimeSpan _maxStep = TimeSpan.FromMinutes(3);

    private readonly TimeSpan _minStep = TimeSpan.FromMinutes(0.5);
    private readonly TimeSpan _timeStep;

    private readonly DateTime _unixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public TotpBase(double period = DefaultPeriod)
    {
        _timeStep = TimeSpan.FromMinutes(Math.Max(Math.Min(period, _maxStep.TotalMinutes), _minStep.TotalMinutes));
    }

    public override async Task<string> GenerateTotpAsync(string secret, double digits = DefaultDigits)
    {
        var secretBytes = _encoding.GetBytes(secret);
        var counter = GetCurrentCounter();
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian) Array.Reverse(counterBytes);

        var hmac = new HMACSHA1(secretBytes);
        var hash = hmac.ComputeHash(counterBytes);

        var offset = hash[^1] & 0xf;
        var selectedBytes = new byte[4];
        Array.Copy(hash, offset, selectedBytes, 0, 4);
        if (BitConverter.IsLittleEndian) Array.Reverse(selectedBytes);

        var selectedInteger = BitConverter.ToInt32(selectedBytes, 0);
        var otp = selectedInteger & 0x7fffffff;
        otp %= (int)Math.Pow(10, digits);
        return await Task.FromResult(otp.ToString().PadLeft((int)digits, '0'));
    }

    public override async Task<bool> ValidateTotpAsync(string secret, string totpValue, double digits = DefaultDigits)
    {
        var otpValid = int.TryParse(totpValue, out var otpValueInt);
        if (!otpValid) return false;

        var generatedTotp = await GenerateTotpAsync(secret, digits);
        return await Task.FromResult(otpValueInt == int.Parse(generatedTotp));
    }

    public override Task<TimeSpan> GetRemainingTimeAsync()
    {
        var remainingTime = GetRemainingTimeUntilNextStep();
        return Task.FromResult(remainingTime.remainingTime);
    }

    public override Task<TimeSpan> GetValidityDurationAsync()
    {
        var remainingTime = GetRemainingTimeUntilNextStep();
        return Task.FromResult(remainingTime.validityDuration);
    }

    private long GetCurrentCounter()
    {
        var utcNow = DateTime.UtcNow;
        var delta = utcNow - _unixEpoch;
        var counter = (long)(delta.TotalSeconds / _timeStep.TotalSeconds);
        return counter;
    }

    private (TimeSpan remainingTime, TimeSpan validityDuration) GetRemainingTimeUntilNextStep()
    {
        var currentTime = DateTime.UtcNow;
        var nextStepTime = _unixEpoch.AddTicks(_timeStep.Ticks * (GetCurrentCounter() + 1));

        if (nextStepTime <= currentTime) return (TimeSpan.Zero, _timeStep);

        var remainingTime = nextStepTime - currentTime;
        return (remainingTime, _timeStep);
    }
}