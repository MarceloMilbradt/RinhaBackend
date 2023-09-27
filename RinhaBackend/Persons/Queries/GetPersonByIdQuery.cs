using Mediator;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Queries;

public sealed record GetPersonByIdQuery(Guid Id) : IRequest<Person?>
{
    public static GetPersonByIdQuery FromId(Guid id) => new(id);
}


public sealed class GetPersonByIdQueryHandler(RedisCacheService redisCacheSevice, IPersonRepository repository) : IRequestHandler<GetPersonByIdQuery, Person?>
{
    public async ValueTask<Person?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await redisCacheSevice.GetItemAsync(request.Id);
        if (person != null)
        {
            return person;
        }
        return await repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
