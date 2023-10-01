using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using RinhaBackend.Application;
using System.IO.Compression;


var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddRequiredServices();

var app = builder.Build();
app.UseResponseCompression();


var pessoasApi = app.MapGroup("/pessoas");

pessoasApi.MapGet("/", async ([FromQuery] string? t, CancellationToken token) =>
{
    if (t is null)
    {
        return Results.BadRequest();
    }
    return Results.Json(await PersonRepository.SearchPersonsAsync(t, token), AppJsonSerializerContext.Default, statusCode: StatusCodes.Status200OK);
});

var getById = "GetById";
pessoasApi.MapGet("/{id:Guid}", async ([FromRoute] Guid id, [FromServices] PersonRepository repository, CancellationToken token) =>
{
    if (id == Guid.Empty)
    {
        return Results.BadRequest();
    }
    return Results.Json(await repository.GetByIdAsync(id, token), AppJsonSerializerContext.Default, statusCode: StatusCodes.Status200OK);
}).WithName(getById);


app.MapGet("/contagem-pessoas", async () =>
{
    return Results.Ok(await PersonRepository.CountPersonsAsync());
});

pessoasApi.MapPost("/", async ([FromBody] Person person, [FromServices] PersonService service, CancellationToken token) =>
{
    var canCreate = await service.ValidateAsync(person);
    if (!canCreate) return Results.UnprocessableEntity();
    try
    {
        var id = await service.Create(person, token);
        var routeValues = new RouteValueDictionary() { ["id"] = id };
        return Results.CreatedAtRoute(getById, routeValues);
    }
    catch
    {
        return Results.UnprocessableEntity();
    }
});



app.Run();