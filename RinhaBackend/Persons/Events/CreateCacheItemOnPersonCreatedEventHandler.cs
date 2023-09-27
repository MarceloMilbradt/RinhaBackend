﻿using Mediator;
using RinhaBackend.Cache;

namespace RinhaBackend.Persons.Events;

public sealed record CreateCacheItemOnPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
{
    private readonly RedisCacheService _cacheService;
    public CreateCacheItemOnPersonCreatedEventHandler(RedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async ValueTask Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
    {
        var person = notification.Person;
        await _cacheService.SetAsync(person);
        await _cacheService.SetKeyAsync(person.Apelido!);
    }
}
