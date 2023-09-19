using RinhaBackend.Models;
using RinhaBackend.Persons.Commands;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonExtensions
{
    public static IServiceCollection AddCompileTimeJsonSerializers(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });
        return services;
    }

}
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(CreatePersonCommand))]
[JsonSerializable(typeof(IEnumerable<Person>))]
[JsonSerializable(typeof(int))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}