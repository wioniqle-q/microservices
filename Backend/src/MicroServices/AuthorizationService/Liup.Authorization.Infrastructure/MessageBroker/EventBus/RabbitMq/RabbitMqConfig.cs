namespace Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class RabbitMqConfig
{
    public RabbitMqConfig(string? hostName, string? userName, string? password, string? queueName)
    {
        HostName = hostName;
        UserName = userName;
        Password = password;
        QueueName = queueName;
    }

    public string? HostName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? QueueName { get; set; }
}
