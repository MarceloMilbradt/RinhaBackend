using Mediator;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using System.Text;

namespace RinhaBackend.Persons.Queries;


public record GetPersonsByTermQuery(string Term) : IRequest<List<Person>>
{
    public static GetPersonsByTermQuery FromTerm(string term)
    {
        return new GetPersonsByTermQuery(term);
    }
}


public sealed class GetPersonsByTermHandler(IPersonRepository repository) : IRequestHandler<GetPersonsByTermQuery, List<Person>>
{
    public async ValueTask<List<Person>> Handle(GetPersonsByTermQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByTermAsync(request.Term, cancellationToken);
    }
}
