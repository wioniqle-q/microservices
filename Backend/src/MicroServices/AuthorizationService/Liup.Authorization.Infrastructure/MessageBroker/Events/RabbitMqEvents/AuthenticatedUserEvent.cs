namespace Liup.Authorization.Infrastructure.MessageBroker.Events.RabbitMqEvents;

public sealed class AuthenticatedUserEvent
{
    //public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    //public string? Token { get; set; }
    //public DateTime? Expiration { get; set; }
}

