using MediatR;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using System.Text;

namespace RinhaBackend.Persons.Queries;


internal sealed record GetPersonsByTermQuery(string Term) : IRequest<IEnumerable<Person>>
{
    public static GetPersonsByTermQuery FromTerm(string term)
    {
        var normalizedText = term.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return new GetPersonsByTermQuery(normalizedText);
    }
}


internal sealed class GetPersonsByTermHandler : IRequestHandler<GetPersonsByTermQuery, IEnumerable<Person>>
{
    private readonly PersonContext _context;
    public GetPersonsByTermHandler(PersonContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Person>> Handle(GetPersonsByTermQuery request, CancellationToken cancellationToken)
    {

        var persons = new List<Person>(500);
        await foreach (var item in PersonContext.SearchPersonsCompiledQueryAsync(_context, request.Term))
        {
            persons.Add(item);
        }
        return persons;
    }
}
