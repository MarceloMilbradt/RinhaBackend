using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

public interface IPersonRepository
{
    Task BulkInsertAsync(IEnumerable<Person> persons, CancellationToken token);
    Task<int> Count();
    Task Create(Person person, CancellationToken token);
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Person>> GetByTermAsync(string term, CancellationToken token);
}