using MemoryPack;
using RinhaBackEndNativeAot.Models;
using StackExchange.Redis;

namespace RinhaBackEndNativeAot.Cache;
internal sealed class RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
{
    public async ValueTask<bool> KeyExistsAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        var db = connectionMultiplexer.GetDatabase();
        bool exists = await db.KeyExistsAsync(key);
        return exists;
    }

    public async ValueTask SetKeyAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var db = connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, 0, TimeSpan.FromHours(5));
    }

    public async ValueTask<Person?> GetItemAsync(Guid key)
    {
        var db = connectionMultiplexer.GetDatabase(1);
        var item = await db.StringGetAsync(key.ToString());
        if (item.HasValue)
        {
            return MemoryPackSerializer.Deserialize<Person>((byte[])item);
        }
        return null;
    }

    public async ValueTask SetAsync(Person value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var db = connectionMultiplexer.GetDatabase(1);
        var bin = MemoryPackSerializer.Serialize(value);
        await db.StringSetAsync(value.Id.ToString(), bin, TimeSpan.FromHours(5));
    }
}
