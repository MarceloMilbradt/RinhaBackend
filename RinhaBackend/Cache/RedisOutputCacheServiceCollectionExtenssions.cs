using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RinhaBackend.Cache;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisOutputCacheServiceCollectionExtenssions
{
    public static IServiceCollection AddRedisOutputCache(this IServiceCollection services)
    {
        services.AddOutputCache();
        services.RemoveAll<IOutputCacheStore>();
        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
        return services;
    }

}