namespace Auth.Infrastructure.UserTransaction.Interfaces;

public interface IOutCome
{
    string? UniqueResId { get; set; }
    string? Description { get; set; }
    string? SubTopic { get; set; }
    string? RsaPublicKey { get; set; }
    string? AccessToken { get; set; }
    string? RefreshToken { get; set; }
    string? ClientAccessToken { get; set; }
    string? DeviceId { get; set; }
    string? ExceptionId { get; set; }
    string? ExceptionType { get; set; }
}

/*
 * access token
refresh token
client access token
    description
sub topic
rsa public key
*/