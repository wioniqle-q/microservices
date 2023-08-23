using Coa.Snowflake.SnowflakeInterfaces;

namespace Coa.Snowflake.SnowflakeAbstractions;

public abstract class SnowflakeAbstract : ISnowflake
{
    private const int WorkerIdShift = 12; // 12 that means 4096 worker id (formula is 2^12)
    private const int DataCenterIdShift = 17; // 12 + 5 that means 32 data center id (formula is 2^5)
    private const int TimestampLeftShift = 22; // 5 + 5 + 12 that means 4096 sequence (formula is 2^12)
    private const int SequenceMask = 4095; // 4096 - 1
    private const int MaxWorkerId = -1 ^ (-1 << 5); // 5 that means 31 worker id
    private const int MaxDataCenterId = -1 ^ (-1 << 5); // 5 that means 31 data center id

    private static readonly DateTimeOffset Epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly object Lock = new(); 
    private static ulong _lastTimestamp; // last timestamp in milliseconds 

    [ThreadStatic] private static long _sequence; // sequence number for current timestamp

    public Task<ulong> GenerateSnowflakeAsync(int dataCenterId, int workerId)
    {
        lock (Lock)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new Exception($"Worker Id can't be greater than {MaxWorkerId} or less than 0");
            if (dataCenterId > MaxDataCenterId || dataCenterId < 0)
                throw new Exception($"Data Center Id can't be greater than {MaxDataCenterId} or less than 0");
            if (_lastTimestamp == TimeGen())
                throw new Exception("Clock moved backwards. Refusing to generate id for 1 milliseconds");
            if (_lastTimestamp > TimeGen())
                throw new Exception(
                    $"Clock moved backwards. Refusing to generate id for {_lastTimestamp - TimeGen()} milliseconds");

            ulong timestamp = TimeGen();
            if (timestamp < _lastTimestamp)
                throw new Exception(
                    $"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                    timestamp = NextMillisecond(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            ulong id = ((timestamp - (ulong)Epoch.ToUnixTimeMilliseconds()) << TimestampLeftShift) |
                     (uint)(dataCenterId << DataCenterIdShift) |
                     (uint)(workerId << WorkerIdShift) | (ulong)_sequence;
            return Task.FromResult(id);
        }
    }

    private static ulong NextMillisecond(ulong lastTimestamp)
    {
        ulong timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
            timestamp = TimeGen();
        return timestamp;
    }

    private static ulong TimeGen()
    {
        return (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}