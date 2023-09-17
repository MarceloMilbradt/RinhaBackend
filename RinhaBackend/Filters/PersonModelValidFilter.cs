using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Filters;

public class PersonModelValidFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext,
        EndpointFilterDelegate next)
    {
        var person = efiContext.GetArgument<PersonDto>(0);

        if (string.IsNullOrWhiteSpace(person.Apelido))
            return Results.UnprocessableEntity();

        if (string.IsNullOrWhiteSpace(person.Apelido))
            return Results.UnprocessableEntity();

        if (person.Stack != null && person.Stack.Any(s => string.IsNullOrWhiteSpace(s)))
            return Results.UnprocessableEntity();


        var dbContext = efiContext.HttpContext.RequestServices.GetRequiredService<PersonContext>();
        var hasAnyWithApelido = await PersonContext.AnyPersonsWithNicknameCompiledQueryAsync(dbContext, person.Apelido);
        if (hasAnyWithApelido) return Results.UnprocessableEntity();
        return await next(efiContext);
    }
}