using Auth.Infrastructure.UserTransaction.ConclusionsAbstraction;

namespace Auth.Infrastructure.UserTransaction.Conclusions;

public sealed class OutcomeValue : OutcomeValueAbstract
{
    public OutcomeValue(string outcome, string? uniqueResId, string? description, string? subTopic,
        string? rsaPublicKey, string? accessToken, string? refreshToken, string? clientAccessToken, string? exceptionId,
        string? exceptionType)
    {
        Outcome = outcome;
        UniqueResId = uniqueResId;
        Description = description;
        SubTopic = subTopic;
        RsaPublicKey = rsaPublicKey;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ClientAccessToken = clientAccessToken;
        ExceptionId = exceptionId;
        ExceptionType = exceptionType;
    }

    public OutcomeValue()
    {
    }

    public override string? Outcome { get; set; }
    public override string? UniqueResId { get; set; }
    public override string? Description { get; set; }
    public override string? SubTopic { get; set; }
    public override string? RsaPublicKey { get; set; }
    public override string? AccessToken { get; set; }
    public override string? RefreshToken { get; set; }
    public override string? ClientAccessToken { get; set; }
    public override string? ExceptionId { get; set; }
    public override string? ExceptionType { get; set; }
}