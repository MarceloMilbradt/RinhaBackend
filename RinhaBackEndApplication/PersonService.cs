namespace RinhaBackend.Application;

public sealed class PersonService(RedisCacheService redisCacheService, GlobalQueue queue)
{
    public async ValueTask<bool> ValidateAsync(Person person)
    {

        if (string.IsNullOrWhiteSpace(person.Apelido) || string.IsNullOrWhiteSpace(person.Nome))
            return false;
        
        if (person.Stack?.Any(s => string.IsNullOrWhiteSpace(s)) ?? false)
            return false;

        if (await redisCacheService.KeyExistsAsync(person.Apelido))
        {
            return false;
        }

        return true;
    }

    public async Task<Guid> Create(Person person, CancellationToken token)
    {
        person.Stack ??= [];
        queue.Enqueue(person);
        await Task.WhenAll(redisCacheService.SetKeyAsync(person.Apelido!), redisCacheService.SetAsync(person));
        return person.Id;
    }

}
