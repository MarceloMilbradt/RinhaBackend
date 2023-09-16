using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Endpoints;

internal static class PersonsEndpointDefinitions
{
    public static void UsePessoasEndpoints(this WebApplication application)
    {
        var endpointGroup = application.MapGroup("pessoas");
        endpointGroup.MapGet("{id}", GetPersonById).WithName("GetPersonById");
        endpointGroup.MapGet("", GetPersons);
        endpointGroup.MapPost("", CreatePersons);
        application.MapGet("contagem-pessoas", CountPersons);
    }

    private static async Task<Ok<int>> CountPersons(PersonContext dbcontext, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await dbcontext.Persons.CountAsync(cancellationToken));
    }

    private static async ValueTask<Results<CreatedAtRoute, BadRequest>> CreatePersons(Person person, PersonContext dbcontext, CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(person.Apelido) ||
                  string.IsNullOrWhiteSpace(person.Nome) ||
                  await dbcontext.Persons.AnyAsync(p => p.Apelido == person.Apelido) ||
                  (person.Stack != null && person.Stack.Any(s => string.IsNullOrWhiteSpace(s))))
        {
            return TypedResults.BadRequest();
        }


        await dbcontext.Persons.AddAsync(person);

        return TypedResults.CreatedAtRoute(nameof(GetPersonById), new { id = person.Id });
    }

    private static async ValueTask<Results<Ok<List<Person>>, BadRequest>> GetPersons(string t, PersonContext dbcontext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(t))
        {
            return TypedResults.BadRequest();
        }

        var persons = await dbcontext
            .Persons.Where(c => c.SearchField.Contains(t, StringComparison.InvariantCultureIgnoreCase))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(persons);
    }

    private static async Task<Results<Ok<Person>, NotFound>> GetPersonById(Guid id, PersonContext dbcontext, CancellationToken cancellationToken)
    {
        var person = await dbcontext.Persons.FindAsync(id, cancellationToken);
        if (person is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(person);
    }
}
