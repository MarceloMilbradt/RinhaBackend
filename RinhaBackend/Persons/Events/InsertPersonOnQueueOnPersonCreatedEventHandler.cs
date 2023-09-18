using MediatR;
using RinhaBackend.Cache;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Events;

internal sealed record InsertPersonOnQueueOnPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
{
    private readonly PersonInsertQueue _queue;
    public InsertPersonOnQueueOnPersonCreatedEventHandler(PersonInsertQueue queue)
    {
        _queue = queue;
    }

    public Task Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
    {
        var person = notification.Person;
        _queue.Enqueue(person);
        return Task.CompletedTask;
    }
}