﻿using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Filters;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Commands;
using RinhaBackend.Persons.Queries;

namespace RinhaBackend.Endpoints;

internal static class PersonsEndpointDefinitions
{
    public static void UsePessoasEndpoints(this WebApplication application)
    {
        var endpointGroup = application.MapGroup("pessoas");

        endpointGroup.MapGet("{id}", GetPersonById).WithName("GetPersonById");

        endpointGroup.MapGet(string.Empty, GetPersons).WithName("GetPersons");

        endpointGroup.MapPost(string.Empty, CreatePersons);

        application.MapGet("contagem-pessoas", CountPersons);

    }

    private static async Task<Ok<int>> CountPersons(PersonContext dbcontext, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await PersonContext.CountPersonsCompiledQueryAsync(dbcontext));
    }
    private static string PersonCreatedAtRouteName = nameof(GetPersonById);
    private static async ValueTask<Results<CreatedAtRoute, BadRequest, UnprocessableEntity>> CreatePersons(
        [FromBody] CreatePersonCommand createPersonCommand, [FromServices]ISender sender, CancellationToken cancellationToken)
    {

        try
        {
            var result = await sender.Send(createPersonCommand, cancellationToken);
            if (!result.CanCreate)
            {
                return TypedResults.UnprocessableEntity();
            }
            return TypedResults.CreatedAtRoute(PersonCreatedAtRouteName, new { id = result.Id });
        }
        catch (DbUpdateException)
        {
            return TypedResults.UnprocessableEntity();
        }
    }


    private static async ValueTask<Results<Ok<IEnumerable<Person>>, BadRequest>> GetPersons([FromQuery] string? t, [FromServices] ISender sender, CancellationToken cancellationToken)
    {
        if (t is null)
        {
            return TypedResults.BadRequest();
        }

        var persons = await sender.Send(GetPersonsByTermQuery.FromTerm(t), cancellationToken);
        return TypedResults.Ok(persons);
    }

    private static async Task<Results<Ok<Person>, NotFound>> GetPersonById([FromRoute] Guid id, [FromServices] ISender sender, CancellationToken cancellationToken)
    {
        var person = await sender.Send(GetPersonByIdQuery.FromId(id), cancellationToken);
        if (person is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(person);
    }
}
