using MediatR;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using System.Text;

namespace RinhaBackend.Persons.Queries;


internal sealed record GetPersonsByTermQuery(string Term) : IRequest<List<Person>>
{
    public static GetPersonsByTermQuery FromTerm(string term)
    {
        var normalizedText = term.RemoveNullBytes().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return new GetPersonsByTermQuery(normalizedText);
    }
}


internal sealed class GetPersonsByTermHandler(PersonContext context) : IRequestHandler<GetPersonsByTermQuery, List<Person>>
{
    public Task<List<Person>> Handle(GetPersonsByTermQuery request, CancellationToken cancellationToken)
    {
        return context.Persons.AsNoTracking().Where(c => c.SearchField!.Contains(request.Term)).Take(50).ToListAsync(cancellationToken: cancellationToken);
    }
}
