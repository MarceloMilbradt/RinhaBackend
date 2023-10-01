using RinhaBackend.Application;
using StackExchange.Redis;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
#if DEBUG
        services.AddSingleton<IConnectionMultiplexer>(
            s => ConnectionMultiplexer.Connect("localhost"));
#else
        services.AddSingleton<IConnectionMultiplexer>(
            s => ConnectionMultiplexer.Connect("localhost"));
#endif
        services.AddSingleton<PersonRepository>();
        services.AddSingleton<PersonService>();
        services.AddSingleton<RedisCacheService>();
        services.AddSingleton<GlobalQueue>();
        services.AddHostedService<BulkInsertWorker>();
        return services;
    }

}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(List<Person>))]
[JsonSerializable(typeof(IEnumerable<Person>))]
[JsonSerializable(typeof(int))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

}