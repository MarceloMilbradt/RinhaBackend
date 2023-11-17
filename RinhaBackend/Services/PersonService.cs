using LazyCache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Services;


public sealed class PersonService
{
    private readonly RedisCacheService _redisCacheService;
    private readonly PersonContext _context;
    private readonly GlobalQueue _insertQueue;
    private readonly ILogger<PersonService> _logger;
    private readonly IAppCache _personLocalCache;
    public PersonService(RedisCacheService redisCacheService, PersonContext context, GlobalQueue insertQueue, ILogger<PersonService> logger)
    {
        _redisCacheService = redisCacheService;
        _context = context;
        _insertQueue = insertQueue;
        _logger = logger;
    }
    public async ValueTask<bool> ValidateAsync(Person person)
    {
        if (string.IsNullOrWhiteSpace(person.Apelido) || string.IsNullOrWhiteSpace(person.Nome))
            return false;

        if (person.Apelido.Length > 32)
            return false;

        if (person.Nome.Length > 100)
            return false;

        if (person.Stack?.Any(s => string.IsNullOrWhiteSpace(s) || s.Length > 32) ?? false)
        {
            return false;
        }

        if (await _redisCacheService.KeyExistsAsync(person.Apelido))
        {
            return false;
        }

        return true;
    }

    public async Task<Guid> CreateAsync(Person person, CancellationToken token)
    {
        person.Stack ??= [];
        _insertQueue.Enqueue(person);
         _personLocalCache.Add(person.Id.ToString(), person, TimeSpan.FromSeconds(120));
        await _redisCacheService.SetAsync(person);
        return person.Id;
    }

    public async ValueTask<Person> GetByIdAsync(Guid id, CancellationToken token)
    {
        var person = _personLocalCache.Get<Person>(id.ToString());
        if(person != null)
        {
            return person;
        }

        person = await _redisCacheService.GetItemAsync(id);
        person ??= await _context.Persons.FindAsync([id], cancellationToken: token);
        return person;
    }
}