using Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;

namespace Auth.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;

public sealed class AssessedEventConsumer : IConsumer<AssessedEvent>
{
    public async Task Consume(ConsumeContext<AssessedEvent> context)
    {
        await Task.CompletedTask;
    }
}