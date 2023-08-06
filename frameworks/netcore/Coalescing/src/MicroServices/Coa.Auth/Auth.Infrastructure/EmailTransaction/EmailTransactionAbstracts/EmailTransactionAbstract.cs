using Auth.Infrastructure.EmailTransaction.EmailTransactionInterfaces;

namespace Auth.Infrastructure.EmailTransaction.EmailTransactionAbstracts;

public abstract class EmailTransactionAbstract : IETransaction
{
    public abstract Task SendEmailAsync(string email, string subject, string message);
    public abstract Task<string> VerifyEmailAsync(string userName, string signature, string publicKey);
}