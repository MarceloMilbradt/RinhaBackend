using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Policies;
using System.Text;
using System.Threading;

namespace RinhaBackend.Endpoints;

internal static class PersonsEndpointDefinitions
{
    public static void UsePessoasEndpoints(this WebApplication application)
    {
        var endpointGroup = application.MapGroup("pessoas");

        endpointGroup.MapGet("{id}", GetPersonById)
            .CacheOutput(x => x.AddPolicy<ByIdCachePolicy>())
            .WithName("GetPersonById");

        endpointGroup.MapGet(string.Empty, GetPersons)
            .CacheOutput(x => x.Tag("pessoas")
            .SetVaryByQuery("t"))
            .WithName("GetPersons");

        endpointGroup.MapPost(string.Empty, CreatePersons);

        application.MapGet("contagem-pessoas", CountPersons);
    }

    private static async Task<Ok<int>> CountPersons(PersonContext dbcontext, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await PersonContext.CountPersonsCompiledQueryAsync(dbcontext));
    }

    private static async ValueTask<Results<CreatedAtRoute, BadRequest>> CreatePersons(PersonDto person, PersonContext dbcontext, IOutputCacheStore cacheStore, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(person.Apelido) ||
            string.IsNullOrWhiteSpace(person.Nome) ||
            (person.Stack != null && person.Stack.Any(s => string.IsNullOrWhiteSpace(s))))
        {
            return TypedResults.BadRequest();
        }

        Person dbPerson;
        if (person.Id == Guid.Empty)
        {
            dbPerson = new Person(person);
            dbcontext.Persons.Add(dbPerson);
        }
        else
        {
            dbPerson = await PersonContext.FindPersonByIdCompiledQueryAsync(dbcontext, person.Id);

            if (dbPerson == null)
            {
                return TypedResults.BadRequest();
            }

            dbPerson.Apelido = person.Apelido;
            dbPerson.Nome = person.Nome;
            dbPerson.Stack = person.Stack.ToList();
            dbPerson.Nascimento = person.Nascimento;
        }

        try
        {
            await dbcontext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return TypedResults.BadRequest();
        }

        await cacheStore.EvictByTagAsync("pessoas", cancellationToken);
        await cacheStore.EvictByTagAsync(person.Id.ToString(), cancellationToken);

        return TypedResults.CreatedAtRoute(nameof(GetPersonById), new { id = dbPerson.Id });
    }

    private static async ValueTask<Results<Ok<List<Person>>, BadRequest>> GetPersons(string? t, PersonContext dbcontext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(t))
        {
            return TypedResults.BadRequest();
        }

        var normalizedText = t.ToLowerInvariant().Normalize(NormalizationForm.FormD);

        var persons = await PersonContext.SearchPersonsCompiledQueryAsync(dbcontext, normalizedText);

        return TypedResults.Ok(persons);
    }

    private static async Task<Results<Ok<Person>, NotFound>> GetPersonById(Guid id, PersonContext dbcontext, CancellationToken cancellationToken)
    {
        var person = await PersonContext.FindPersonByIdCompiledQueryAsync(dbcontext, id);
        if (person is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(person);
    }
}
