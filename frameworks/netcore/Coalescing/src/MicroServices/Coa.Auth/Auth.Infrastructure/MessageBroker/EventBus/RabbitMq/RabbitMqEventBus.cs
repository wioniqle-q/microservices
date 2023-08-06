using MassTransit;

namespace Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class RabbitMqEventBus : IEventBus
{
    private readonly IBusControl _busControl;

    public RabbitMqEventBus(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public void Publish<T>(T @event, CancellationToken cancellationToken) where T : class
    {
        _busControl.Publish(@event, cancellationToken);
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : class
    {
        return _busControl.Publish(@event, cancellationToken);
    }
}