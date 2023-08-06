using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;

namespace Auth.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;

public sealed class InsertedEventConsumer : IConsumer<InsertedEvent>
{
    public async Task Consume(ConsumeContext<InsertedEvent> context)
    {
        await Task.CompletedTask;
    }
}