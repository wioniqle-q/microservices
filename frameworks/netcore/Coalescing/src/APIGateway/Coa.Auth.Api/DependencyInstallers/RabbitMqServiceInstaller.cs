using Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;
using Auth.Infrastructure.MessageBroker.EventHandler.RabbitMqEventHandler;
using MassTransit;

namespace Coa.Auth.Api.DependencyInstallers;

public static class RabbitMqServiceInstaller
{
    public static void InstallMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new RabbitMqOptions(
            configuration.GetSection("RabbitMqConfig:HostName").Value ?? string.Empty,
            configuration.GetSection("RabbitMqConfig:UserName").Value ?? string.Empty,
            configuration.GetSection("RabbitMqConfig:Password").Value ?? string.Empty,
            configuration.GetSection("RabbitMqConfig:VirtualHost").Value ?? string.Empty,
            configuration.GetSection("RabbitMqConfig:QueueName1").Value ?? string.Empty
        );

        var baseUrlOptions = new BaseUrlOptions(
            configuration.GetSection("RabbitMqConfig:HostUrl").Value ?? string.Empty
        );
        services.AddSingleton(baseUrlOptions);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<AssessedEventConsumer>();
            x.AddConsumer<PwResetEventConsumer>();
            x.AddConsumer<UnAuthorisedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.HostName, options.VirtualHost, h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });

                cfg.ReceiveEndpoint(options.QueueName, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
                    e.ConfigureConsumer<AssessedEventConsumer>(context);
                    e.ConfigureConsumer<PwResetEventConsumer>(context);
                    e.ConfigureConsumer<UnAuthorisedEventConsumer>(context);
                });
            });
        });

        services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
        services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
        services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());

        services.AddSingleton<IEventBus, RabbitMqEventBus>();
    }
}