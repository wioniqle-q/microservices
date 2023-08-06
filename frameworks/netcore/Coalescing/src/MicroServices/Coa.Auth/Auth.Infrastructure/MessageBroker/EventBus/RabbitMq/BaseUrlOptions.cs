namespace Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class BaseUrlOptions
{
    public BaseUrlOptions(string host)
    {
        Host = host;
    }

    public string Host { get; }
}