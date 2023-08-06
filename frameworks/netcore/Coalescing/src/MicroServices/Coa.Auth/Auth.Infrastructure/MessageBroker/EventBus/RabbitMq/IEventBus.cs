namespace Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;

public interface IEventBus
{
    void Publish<T>(T @event, CancellationToken cancellationToken) where T : class;
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : class;
}