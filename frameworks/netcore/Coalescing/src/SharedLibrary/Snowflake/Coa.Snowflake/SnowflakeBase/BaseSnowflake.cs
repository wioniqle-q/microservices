using Coa.Snowflake.SnowflakeAbstractions;

namespace Coa.Snowflake.SnowflakeBase;

public sealed class BaseSnowflake : SnowflakeAbstract
{
    public new async Task<ulong> GenerateSnowflakeAsync(int dataCenterId, int workerId)
    {
        return await base.GenerateSnowflakeAsync(dataCenterId, workerId);
    }
}