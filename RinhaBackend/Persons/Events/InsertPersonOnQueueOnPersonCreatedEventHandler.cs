﻿using MediatR;
using RinhaBackend.Cache;

namespace RinhaBackend.Persons.Events;

internal sealed record InsertPersonOnQueueOnPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
{
    private readonly IRedisCacheSevice _cacheService;
    public InsertPersonOnQueueOnPersonCreatedEventHandler(IRedisCacheSevice cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
    {
        var person = notification.Person;
        await _cacheService.SetAsync(person.Id, person);
        await _cacheService.SetKeyAsync(person.Apelido);
    }
}