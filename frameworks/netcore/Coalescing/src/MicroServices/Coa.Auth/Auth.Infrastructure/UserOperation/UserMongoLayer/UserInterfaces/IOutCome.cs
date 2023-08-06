namespace Auth.Infrastructure.UserOperation.UserMongoLayer.UserInterfaces;

public interface IOutCome
{
    string? UniqueResId { get; set; }
    string? Description { get; set; }
    string? SubTopic { get; set; }
    string? RsaPublicKey { get; set; }
    string? AccessToken { get; set; }
    string? RefreshToken { get; set; }
    string? ClientAccessToken { get; set; }
}