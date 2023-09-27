using Mediator;
using RinhaBackend.Cache;
using RinhaBackend.Persistence;

namespace RinhaBackend.Persons.Events;

//public sealed record InsertPersonOnQueueOnPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
//{
//    private readonly PersonInsertQueue _queue;
//    public InsertPersonOnQueueOnPersonCreatedEventHandler(PersonInsertQueue queue)
//    {
//        _queue = queue;
//    }

//    public ValueTask Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
//    {
//        var person = notification.Person;
//        _queue.Enqueue(person);
//        return ValueTask.CompletedTask;
//    }
//}

public sealed record InsertPersonOnDbOnPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
{
    private readonly IPersonRepository _repository;
    public InsertPersonOnDbOnPersonCreatedEventHandler(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
    {
        var person = notification.Person;
        await _repository.Create(person, cancellationToken);
    }
}