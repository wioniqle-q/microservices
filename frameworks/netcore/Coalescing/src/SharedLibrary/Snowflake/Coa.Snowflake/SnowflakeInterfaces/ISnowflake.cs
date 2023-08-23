namespace Coa.Snowflake.SnowflakeInterfaces;

public interface ISnowflake
{
    Task<ulong> GenerateSnowflakeAsync(int dataCenterId, int workerId);
}