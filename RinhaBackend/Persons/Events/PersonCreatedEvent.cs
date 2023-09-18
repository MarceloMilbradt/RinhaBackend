using MediatR;
using RinhaBackend.Models;
using StackExchange.Redis;

namespace RinhaBackend.Persons.Events;

internal sealed record PersonCreatedEvent(Person Person) : INotification
{
    public static PersonCreatedEvent From(Person person) => new(person);
}
