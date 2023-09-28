using Microsoft.AspNetCore.Mvc;
using RinhaBackend.Application;


var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddRequiredServices();

var app = builder.Build();



var pessoasApi = app.MapGroup("/pessoas");

pessoasApi.MapGet("/", async ([FromQuery] string? t, CancellationToken token) =>
{
    if (t is null)
    {
        return Results.BadRequest();
    }
    return Results.Ok(await PersonRepository.SearchPersonsAsync(t, token));
});

var getById = "GetById";
pessoasApi.MapGet("/{id:Guid}", async ([FromRoute] Guid id, [FromServices] PersonRepository repository, CancellationToken token) =>
{
    if (id == Guid.Empty)
    {
        return Results.BadRequest();
    }
    return Results.Ok(await repository.GetByIdAsync(id, token));
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