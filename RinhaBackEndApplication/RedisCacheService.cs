using MemoryPack;
using StackExchange.Redis;

namespace RinhaBackend.Application;
public sealed class RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
{
    public async Task<Person?> GetItemAsync(Guid key)
    {
        var db = connectionMultiplexer.GetDatabase(1);
        var item = await db.StringGetAsync(key.ToString());
        if (item.HasValue)
        {
            Person _cachedPerson = new();
            MemoryPackSerializer.Deserialize((byte[])item!, ref _cachedPerson!);
            return _cachedPerson;
        }
        return null;
    }

    public async Task SetAsync(Person value)
    {
        var db = connectionMultiplexer.GetDatabase(1);
        var bin = MemoryPackSerializer.Serialize(value);
        await db.StringSetAsync(value.Id.ToString(), bin, TimeSpan.FromMinutes(10));
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        var db = connectionMultiplexer.GetDatabase();
        bool exists = await db.KeyExistsAsync(key);
        return exists;
    }

    public async Task SetKeyAsync(string key)
    {
        var db = connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, 0, TimeSpan.FromMinutes(5));
    }
}
