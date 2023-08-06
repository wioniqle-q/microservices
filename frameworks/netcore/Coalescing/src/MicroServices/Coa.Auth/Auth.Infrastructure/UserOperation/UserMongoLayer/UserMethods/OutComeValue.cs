using Auth.Infrastructure.UserOperation.UserMongoLayer.UserAbstractions;

namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserMethods;

public sealed class OutComeValue : OutcomeValueAbstract
{
    public OutComeValue(string? uniqueResId, string? description, string? subTopic, string? rsaPublicKey,
        string? accessToken, string? refreshToken, string? clientAccessToken)
    {
        UniqueResId = uniqueResId;
        Description = description;
        SubTopic = subTopic;
        RsaPublicKey = rsaPublicKey;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ClientAccessToken = clientAccessToken;
    }

    public OutComeValue()
    {
    }

    public override string? UniqueResId { get; set; }
    public override string? Description { get; set; }
    public override string? SubTopic { get; set; }
    public override string? RsaPublicKey { get; set; }
    public override string? AccessToken { get; set; }
    public override string? RefreshToken { get; set; }
    public override string? ClientAccessToken { get; set; }
}