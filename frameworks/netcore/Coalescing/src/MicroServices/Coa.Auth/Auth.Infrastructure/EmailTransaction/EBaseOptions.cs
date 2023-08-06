namespace Auth.Infrastructure.EmailTransaction;

public sealed class EBaseOptions
{
    public EBaseOptions(string fromMail, string fromMailPassword, string smtpServer, int smtpPort, bool enableSsl)
    {
        FromMail = fromMail;
        FromMailPassword = fromMailPassword;
        SmtpServer = smtpServer;
        SmtpPort = smtpPort;
        EnableSsl = enableSsl;
    }

    public string FromMail { get; }
    public string FromMailPassword { get; }
    public string SmtpServer { get; }
    public int SmtpPort { get; }
    public bool EnableSsl { get; }
}