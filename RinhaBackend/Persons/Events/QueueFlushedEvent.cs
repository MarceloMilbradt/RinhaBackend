using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Events;

internal sealed record QueueFlushedEvent(IEnumerable<Person> Persons) : INotification;

internal sealed record InsertPersonOnDbQueueFlushedEvent : INotificationHandler<QueueFlushedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InsertPersonOnDbQueueFlushedEvent(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    //public InsertPersonOnDbQueueFlushedEvent(PersonContext context)
    //{
    //    _context = context;
    //}

    public async Task Handle(QueueFlushedEvent notification, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<PersonContext>();
        await _context.BulkInsertAsync(notification.Persons, cancellationToken: cancellationToken);
    }
}