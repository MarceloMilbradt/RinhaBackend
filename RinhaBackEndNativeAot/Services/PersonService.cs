using RinhaBackEndNativeAot.Cache;
using RinhaBackEndNativeAot.Models;
using RinhaBackEndNativeAot.Persistence;
using System.Text;

namespace RinhaBackEndNativeAot.Services;

internal sealed class PersonService(PersonInsertQueue personInsertQueue, RedisCacheService redisCacheService)
{
    private const char TrimChar = '\0';

    public async ValueTask<bool> ValidateAsync(PersonDto personDto)
    {

        if (string.IsNullOrWhiteSpace(personDto.Apelido) || string.IsNullOrWhiteSpace(personDto.Nome))
            return false;
        
        if (personDto.Stack?.Any(s => string.IsNullOrWhiteSpace(s)) ?? false)
            return false;

        return !await redisCacheService.KeyExistsAsync(personDto.Apelido);
    }

    public async Task<Guid> Create(PersonDto personDto)
    {
        var trimmedStackItems = (personDto.Stack ?? [])
            .Select(item => item.TrimEnd(TrimChar))
            .ToList();

        Person newPerson = new()
        {
            Id = Guid.NewGuid(),
            Apelido = personDto.Apelido.TrimEnd(TrimChar),
            Nome = personDto.Nome.TrimEnd(TrimChar),
            Nascimento = personDto.Nascimento,
            Stack = trimmedStackItems,
        };
        newPerson.SearchField = TermGenerator.BuildSearchField(newPerson).ToString();

        personInsertQueue.Enqueue(newPerson);
        await redisCacheService.SetAsync(newPerson);
        return newPerson.Id;
    }

}
