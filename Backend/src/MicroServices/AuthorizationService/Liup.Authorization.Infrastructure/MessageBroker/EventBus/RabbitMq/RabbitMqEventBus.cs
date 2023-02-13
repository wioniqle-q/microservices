using MassTransit;

namespace Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class RabbitMqEventBus : IEventBus
{
    private readonly IBusControl _busControl;

    public RabbitMqEventBus(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public void Publish<T>(T message) where T : class
    {
        _busControl.Publish(message);
    }
}
