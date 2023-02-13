namespace Liup.Authorization.Infrastructure.Data.MongoDB.Options;

public sealed class DataOptions
{
    public DataOptions(string? connectionString, string? database)
    {
        ConnectionString = connectionString;
        Database = database;
    }

    public string? ConnectionString { get; }
    public string? Database { get; }
}
