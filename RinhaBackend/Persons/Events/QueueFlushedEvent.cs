using MediatR;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Events;

public sealed record QueueFlushedEvent(IEnumerable<Person> Persons) : INotification;

internal sealed record InsertPersonOnDbQueueFlushedEvent : INotificationHandler<QueueFlushedEvent>
{
    private readonly PersonContext _context;

    public InsertPersonOnDbQueueFlushedEvent(PersonContext context)
    {
        _context = context;
    }

    public async Task Handle(QueueFlushedEvent notification, CancellationToken cancellationToken)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            await _context.Persons.AddRangeAsync(notification.Persons, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}