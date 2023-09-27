using Mediator;
using RinhaBackend.Models;
using StackExchange.Redis;

namespace RinhaBackend.Persons.Events;

public sealed record PersonCreatedEvent(Person Person) : INotification
{
    public static PersonCreatedEvent From(Person person) => new(person);
}
