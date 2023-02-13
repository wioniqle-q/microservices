
using MassTransit;

namespace Liup.Authorization.Infrastructure.MessageBroker.Consumer.RabbitMq;

public class EventBusConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
{
    public Task Consume(ConsumeContext<TMessage> context)
    {
        var message = context.Message;
        return Task.CompletedTask;
    }
}
