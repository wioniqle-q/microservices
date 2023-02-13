namespace Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;

public interface IEventBus
{
    void Publish<T>(T message) where T : class;
}
