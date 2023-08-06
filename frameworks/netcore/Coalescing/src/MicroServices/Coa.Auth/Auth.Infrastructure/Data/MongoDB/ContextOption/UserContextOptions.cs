namespace Auth.Infrastructure.Data.MongoDB.ContextOption;

public class UserContextOptions
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
    public string CollectionName { get; init; } = null!;
}