using MediatR;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;
using System;
using System.Text;

namespace RinhaBackend.Persons.Commands;
internal sealed record CreatePersonCommand(string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack) : ICreatePersonRequestWithValidation;
internal sealed class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, CreatePersonResult>
{
    private readonly IPublisher _publisher;
    public CreatePersonCommandHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<CreatePersonResult> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var stackItems = request.Stack ?? Array.Empty<string>();
        Person newPerson = new()
        {
            Id = Guid.NewGuid(),
            Apelido = (request.Apelido).TrimEnd('\0'),
            Nome = (request.Nome).TrimEnd('\0'),
            Nascimento = request.Nascimento,
            Stack = stackItems.Select(s => s.TrimEnd('\0')).ToList(),
        };
        BuildSearchField(newPerson);
        await _publisher.Publish(PersonCreatedEvent.From(newPerson), cancellationToken);
        return CreatePersonResult.Success(newPerson.Id);
    }

    public static void BuildSearchField(Person person)
    {
        var sb = new StringBuilder();
        sb.Append(person.Apelido)
           .Append(' ')
           .Append(person.Nome)
           .Append(' ');

        foreach (var item in person.Stack)
        {
            sb.Append(item).Append(' ');
        }
        person.SearchField = sb.ToString().RemoveNullBytes();
    }
}
