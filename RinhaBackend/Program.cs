using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Services;
using RinhaBackend.Persistence;
using RinhaBackend.Models;


var builder = WebApplication.CreateSlimBuilder(args);


builder.Services.AddRequiredServices();

builder.Services.AddDbContextPool<PersonContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("RinhaBackend"))
            .EnableThreadSafetyChecks(false)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
    , 4096);

var app = builder.Build();

var pessoasApi = app.MapGroup("/pessoas");

pessoasApi.MapGet("/", ([FromQuery] string? t, PersonContext context, CancellationToken token) =>
{
    if (t == null)
    {
        return Results.BadRequest();
    }
    try
    {
        return Results.Json(PersonContext.SearchPersonsCompiledQueryAsync(context, t.ToLower()), AppJsonSerializerContext.Default, statusCode: StatusCodes.Status200OK);
    }
    catch
    {
        return Results.Json(new List<Person>(), AppJsonSerializerContext.Default, statusCode: StatusCodes.Status200OK);
    }
});

var getById = "GetById";
pessoasApi.MapGet("/{id:Guid}", async ([FromRoute] Guid id, PersonService service, CancellationToken token) =>
{
    if (id == Guid.Empty)
    {
        return Results.BadRequest();
    }
    return Results.Json(await service.GetByIdAsync(id, token), AppJsonSerializerContext.Default, statusCode: StatusCodes.Status200OK);
}).WithName(getById);


app.MapGet("/contagem-pessoas", async (PersonContext context) =>
{
    return Results.Ok(await PersonContext.CountPersonsCompiledQueryAsync(context));
});

var routeValues = new RouteValueDictionary();
pessoasApi.MapPost("/", async ([FromBody] Person person, [FromServices] PersonService service, CancellationToken token) =>
{
    var canCreate = await service.ValidateAsync(person);
    if (!canCreate) return Results.UnprocessableEntity();
    try
    {
        var id = await service.CreateAsync(person, token);
        routeValues["id"] = id;
        return Results.CreatedAtRoute(getById, routeValues);
    }
    catch
    {
        return Results.UnprocessableEntity();
    }
});


app.Run();