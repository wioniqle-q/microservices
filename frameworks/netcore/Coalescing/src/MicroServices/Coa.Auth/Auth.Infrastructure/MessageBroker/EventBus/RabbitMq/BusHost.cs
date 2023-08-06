using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Auth.Infrastructure.MessageBroker.EventBus.RabbitMq;

public sealed class BusHost : IHostedService
{
    private readonly IBusControl _busControl;

    public BusHost(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _busControl.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _busControl.StopAsync(cancellationToken);
    }
}