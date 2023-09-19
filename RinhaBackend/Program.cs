using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.BackgroundServices;
using RinhaBackend.Cache;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Commands;
using RinhaBackend.Persons.Queries;
using StackExchange.Redis;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSingleton<PersonInsertQueue>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    s => ConnectionMultiplexer.Connect("cache"));

builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddHostedService<PersonDatabaseWorker>();


builder.Services.AddCompileTimeJsonSerializers();
builder.Services.AddMediatR();
builder.Services.AddPersonContext(builder.Configuration.GetConnectionString("RinhaBackend"));


var app = builder.Build();

var pessoasApi = app.MapGroup("/pessoas");

pessoasApi.MapGet("/", async ([FromQuery] string? t, [FromServices] ISender sender, CancellationToken cancellationToken) =>
{
    if (t is null)
    {
        return Results.BadRequest();
    }

    var persons = await sender.Send(GetPersonsByTermQuery.FromTerm(t), cancellationToken);
    return Results.Ok(persons);
});

pessoasApi.MapGet("/{id:Guid}", async ([FromRoute] Guid id, [FromServices] ISender sender, CancellationToken cancellationToken) =>
{
    var person = await sender.Send(GetPersonByIdQuery.FromId(id), cancellationToken);
    if (person is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(person);

}).WithName("GetById");


app.MapGet("/contagem-pessoas", async (PersonContext dbcontext, CancellationToken cancellationToken) =>
{
    return Results.Ok(await PersonContext.CountPersonsCompiledQueryAsync(dbcontext));
});


pessoasApi.MapPost("/", async ([FromBody] CreatePersonCommand createPersonCommand, [FromServices] ISender sender, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await sender.Send(createPersonCommand, cancellationToken);
        if (!result.CanCreate)
        {
            return Results.UnprocessableEntity();
        }
        return Results.CreatedAtRoute("GetById", new { id = result.Id });
    }
    catch (DbUpdateException)
    {
        return Results.UnprocessableEntity();
    }
});




app.InitializeDatabase();
app.Run();