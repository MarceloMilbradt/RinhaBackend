using Mediator;
using RinhaBackend.Models;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Events;

public sealed record QueueFlushedEvent(IEnumerable<Person> Persons) : INotification;

public sealed record InsertPersonOnDbQueueFlushedEvent : INotificationHandler<QueueFlushedEvent>
{
    private readonly IPersonRepository _repository;
    public InsertPersonOnDbQueueFlushedEvent(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask Handle(QueueFlushedEvent notification, CancellationToken cancellationToken)
    {
        await _repository.BulkInsertAsync(notification.Persons,cancellationToken);
    }
}