using Auth.Domain.Entities.MongoEntities;

namespace Auth.Infrastructure.MessageBroker.Events.RabbitMqEvents;

public sealed class UnAuthorisedEvent
{
    public string UserName { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string MiddleName { get; set; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public BaseUserProperty UserProperty { get; init; } = null!;
    public BaseDevice Device { get; init; } = null!;
}