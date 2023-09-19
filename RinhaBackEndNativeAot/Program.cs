using Microsoft.AspNetCore.Mvc;
using RinhaBackEndNativeAot.BackgroundServices;
using RinhaBackEndNativeAot.Cache;
using RinhaBackEndNativeAot.Models;
using RinhaBackEndNativeAot.Persistence;
using RinhaBackEndNativeAot.Services;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddSingleton<PersonInsertQueue>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<PersonRepository>();
builder.Services.AddScoped<RedisCacheService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    s => ConnectionMultiplexer.Connect("cache"));

builder.Services.AddHostedService<PersonDatabaseWorker>();

var app = builder.Build();



var pessoasApi = app.MapGroup("/pessoas");

pessoasApi.MapGet("/", async ([FromQuery] string? t, CancellationToken token) =>
{
    if (t is null)
    {
        return Results.BadRequest();
    }
    var normalizedText = t.TrimEnd('\0').ToLowerInvariant().Normalize(NormalizationForm.FormD);
    return Results.Ok(await PersonRepository.SearchPersonsAsync(normalizedText, token));
});

pessoasApi.MapGet("/{id:Guid}", async ([FromRoute] Guid id, [FromServices] PersonRepository repository, CancellationToken token) =>
{
    if (id == Guid.Empty)
    {
        return Results.BadRequest();
    }
    return Results.Ok(await repository.GetByIdAsync(id, token));
}).WithName("GetById");


pessoasApi.MapGet("/contagem-pessoas", async  () =>
{
    return Results.Ok(await PersonRepository.CountPersonsAsync());
});


pessoasApi.MapPost("/", async ([FromBody] PersonDto person, [FromServices] PersonService service, CancellationToken token) =>
{
    var canCreate = await service.ValidateAsync(person);
    if (!canCreate) return Results.UnprocessableEntity();
    var id = await service.Create(person);
    var routeValues = new RouteValueDictionary() { ["id"] = id };
    return Results.CreatedAtRoute("GetById", routeValues);
});



app.Run();




[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(PersonDto))]
[JsonSerializable(typeof(IEnumerable<Person>))]
[JsonSerializable(typeof(int))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}