using Liup.Authorization.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;
using Liup.Authorization.Infrastructure.MessageBroker.Events.RabbitMqEvents;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Liup.Authorization.Infrastructure.MessageBroker.EventBus.RabbitMq;

public static class RabbitMqConfigure
{
    public static void AddRabbitMq(this IServiceCollection services, RabbitMqConfig configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<AuthenticatedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
               {
                   cfg.Host(configuration.HostName, h =>
                   {
                       h.Username(configuration.UserName);
                       h.Password(configuration.Password);
                   });

                   cfg.ReceiveEndpoint(string.Concat(configuration.QueueName), e =>
                   {
                       e.PrefetchCount = 16;
                       e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
                       e.ConfigureConsumer<AuthenticatedEventConsumer>(context);
                   });
               });
        });

        services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
        services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
        services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());

        services.AddSingleton<AuthenticatedEventConsumer>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddSingleton<AuthenticatedUserEvent>();
    }
}
