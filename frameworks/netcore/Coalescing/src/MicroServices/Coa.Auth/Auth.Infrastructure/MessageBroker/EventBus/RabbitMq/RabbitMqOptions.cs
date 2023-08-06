namespace Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class RabbitMqOptions
{
    public RabbitMqOptions(string hostName, string userName, string password, string virtualHost, string queueName)
    {
        HostName = hostName;
        UserName = userName;
        Password = password;
        VirtualHost = virtualHost;
        QueueName = queueName;
    }

    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }
    public string VirtualHost { get; }
    public string QueueName { get; }
}