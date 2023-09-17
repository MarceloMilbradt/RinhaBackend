using RinhaBackend.Persons.Commands;
using RinhaBackend.Cache;

namespace RinhaBackend.Filters;

public class PersonModelValidFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext,
        EndpointFilterDelegate next)
    {
        var person = efiContext.GetArgument<CreatePersonCommand>(0);

        if (string.IsNullOrWhiteSpace(person.Apelido))
            return Results.UnprocessableEntity();

        if (string.IsNullOrWhiteSpace(person.Nome))
            return Results.UnprocessableEntity();

        if (person.Stack != null && person.Stack.Any(s => string.IsNullOrWhiteSpace(s)))
            return Results.UnprocessableEntity();


        var redisCacheService = efiContext.HttpContext.RequestServices.GetRequiredService<IRedisCacheSevice>();
        var hasAnyWithApelido = await redisCacheService.KeyExistsAsync(person.Apelido);
        if (hasAnyWithApelido) return Results.UnprocessableEntity();
        return await next(efiContext);
    }
}