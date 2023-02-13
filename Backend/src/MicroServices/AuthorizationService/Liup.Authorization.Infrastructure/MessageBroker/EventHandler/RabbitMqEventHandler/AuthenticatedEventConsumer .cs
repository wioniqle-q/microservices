using Liup.Authorization.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;

namespace Liup.Authorization.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;

public sealed class AuthenticatedEventConsumer : IConsumer<AuthenticatedUserEvent>
{
    public Task Consume(ConsumeContext<AuthenticatedUserEvent> context)
    {
        var message = context.Message;
        Console.WriteLine($"AuthenticatedEventConsumer: {message.FirstName} {message.LastName}");
        return Task.CompletedTask;
    }
}