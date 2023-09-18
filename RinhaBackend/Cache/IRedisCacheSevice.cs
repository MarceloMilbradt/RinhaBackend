namespace RinhaBackend.Cache;

public interface IRedisCacheSevice
{
    ValueTask<T> GetItemAsync<T>(Guid key);
    ValueTask<bool> KeyExistsAsync(string key);
    ValueTask SetAsync<T>(Guid key, T value);
    ValueTask SetKeyAsync(string key);
}