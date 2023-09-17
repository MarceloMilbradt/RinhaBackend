using MediatR;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Queries;

internal sealed record GetPersonByIdQuery(Guid Id) : IRequest<Person?>
{
    public static GetPersonByIdQuery FromId(Guid id) => new GetPersonByIdQuery(id);
}


internal sealed class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Person?>
{
    private readonly IRedisCacheSevice _redisCacheSevice;

    private readonly PersonContext _context;
    public GetPersonByIdQueryHandler(IRedisCacheSevice redisCacheSevice, PersonContext context)
    {
        _redisCacheSevice = redisCacheSevice;
        _context = context;
    }

    public async Task<Person?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _redisCacheSevice.GetItemAsync<Person>(request.Id);
        if (person is not null)
        {
            return person;
        }
        return await PersonContext.FindPersonByIdCompiledQueryAsync(_context, request.Id);
    }
}
