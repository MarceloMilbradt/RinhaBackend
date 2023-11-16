using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Services;


public sealed class PersonService
{
    private readonly RedisCacheService _redisCacheService;
    private readonly PersonContext _context;
    private readonly GlobalQueue _insertQueue;
    private readonly ILogger<PersonService> _logger;

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
        await _redisCacheService.SetAsync(person);
        return person.Id;
    }

    public async Task<Person> GetByIdAsync(Guid id, CancellationToken token)
    {
        var person = await _redisCacheService.GetItemAsync(id);
        if (person == null)
        {
            person = await _context.Persons.FindAsync([id], cancellationToken: token);
            if (person != null)
            {
                await _redisCacheService.SetAsync(person);
            }
        }
        return person;
    }

    public static string TruncateString(string input, int maxLength)
    {
        return input.Length <= maxLength ? input : input[..maxLength];
    }
}