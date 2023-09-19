using MediatR;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using System.Text;

namespace RinhaBackend.Persons.Queries;


internal sealed record GetPersonsByTermQuery(string Term) : IRequest<IEnumerable<Person>>
{
    public static GetPersonsByTermQuery FromTerm(string term)
    {
        var normalizedText = term.RemoveNullBytes().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return new GetPersonsByTermQuery(normalizedText);
    }
}


internal sealed class GetPersonsByTermHandler(PersonContext context) : IRequestHandler<GetPersonsByTermQuery, IEnumerable<Person>>
{
    private readonly PersonContext _context = context;

    public async Task<IEnumerable<Person>> Handle(GetPersonsByTermQuery request, CancellationToken cancellationToken)
    {

        var persons = new List<Person>(50);
        await foreach (var item in PersonContext.SearchPersonsCompiledQueryAsync(_context, request.Term))
        {
            persons.Add(item);
        }
        return persons;
    }
}
