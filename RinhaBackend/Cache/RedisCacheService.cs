using Newtonsoft.Json;
using StackExchange.Redis;

namespace RinhaBackend.Cache;

public sealed class RedisCacheService : IRedisCacheSevice
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async ValueTask<bool> KeyExistsAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        var db = _connectionMultiplexer.GetDatabase();
        bool exists = await db.KeyExistsAsync(key);
        return exists;
    }

    public async ValueTask SetKeyAsync(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, 0, TimeSpan.FromHours(5));
    }

    public async ValueTask<T> GetItemAsync<T>(Guid key)
    {
        var db = _connectionMultiplexer.GetDatabase(1);
        var item = await db.StringGetAsync(key.ToString());
        return JsonConvert.DeserializeObject<T>(item)!;
    }

    public async ValueTask SetAsync<T>(Guid key, T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var db = _connectionMultiplexer.GetDatabase(1);
        var item = JsonConvert.SerializeObject(value);
        await db.StringSetAsync(key.ToString(), item, TimeSpan.FromHours(5));
    }
}
