using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Services;
using RinhaBackend.Workers;
using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

#if DEBUG
        services.AddSingleton<IConnectionMultiplexer>(
            s => ConnectionMultiplexer.Connect("localhost"));
#else
        services.AddSingleton<IConnectionMultiplexer>(
            s => ConnectionMultiplexer.Connect("cache"));
#endif
        services.AddScoped<PersonService>();
        services.AddSingleton<RedisCacheService>();
        services.AddSingleton<GlobalQueue>();
        services.AddHostedService<BulkInsertWorker>();
        services.AddHostedService<WarmupWorker>();
        return services;
    }

}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(IAsyncEnumerable<Person>))]
[JsonSerializable(typeof(List<Person>))]
[JsonSerializable(typeof(int))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

}