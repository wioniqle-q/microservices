using Auth.Domain.Entities.MongoEntities;
using Auth.Infrastructure.TransferProtocol.TransferAbstractions;

namespace Auth.Infrastructure.TransferProtocol.TransferUtilitiy;

public sealed class DeviceComparer<T> : DeviceComparerAbstract where T : BaseDevice
{
    public override async Task<List<string>> GetChangedProperties<T1>(T1 oldObject, T1 newObject)
    {
        var properties = typeof(T).GetProperties();

        return await Task.FromResult(properties
            .Where(property =>
            {
                var x = property.GetValue(oldObject);
                var y = property.GetValue(newObject);
                return x is not null && y is not null && x.ToString() != y.ToString();
            })
            .Select(property => property.Name)
            .ToList());
    }

    public override async Task<bool> CompareDevices<T1>(T1 oldObject, T1 newObject)
    {
        var properties = typeof(T).GetProperties();

        return await Task.FromResult(!properties.Any(property =>
        {
            var x = property.GetValue(oldObject);
            var y = property.GetValue(newObject);
            return x is not null && y is not null && x.ToString() != y.ToString();
        }));
    }
}