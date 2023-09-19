using MediatR;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Queries;

internal sealed record GetPersonByIdQuery(Guid Id) : IRequest<Person?>
{
    public static GetPersonByIdQuery FromId(Guid id) => new GetPersonByIdQuery(id);
}


internal sealed class GetPersonByIdQueryHandler(RedisCacheService redisCacheSevice, PersonContext context) : IRequestHandler<GetPersonByIdQuery, Person?>
{
    public async Task<Person?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await redisCacheSevice.GetItemAsync(request.Id);
        if (person is not null)
        {
            return person;
        }
        return await PersonContext.FindPersonByIdCompiledQueryAsync(context, request.Id);
    }
}
