namespace Auth.Infrastructure.EmailTransaction.EmailTransactionInterfaces;

public interface IETransaction
{
    Task SendEmailAsync(string email, string subject, string message);
    Task<string> VerifyEmailAsync(string userName, string signature, string publicKey);
}