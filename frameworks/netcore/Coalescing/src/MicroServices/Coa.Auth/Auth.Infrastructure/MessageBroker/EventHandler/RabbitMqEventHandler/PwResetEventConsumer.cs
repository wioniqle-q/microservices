using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;

namespace Auth.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;

public sealed class PwResetEventConsumer : IConsumer<PwResetEvent>
{
    public async Task Consume(ConsumeContext<PwResetEvent> context)
    {
        await Task.CompletedTask;
    }
}