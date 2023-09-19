using MediatR;
using RinhaBackend.Models;
using RinhaBackend.Persons.Events;

namespace RinhaBackend.Persons.Commands;
internal sealed record CreatePersonCommand(string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack) : ICreatePersonRequestWithValidation;
internal sealed class CreatePersonCommandHandler(IPublisher publisher) : IRequestHandler<CreatePersonCommand, CreatePersonResult>
{
    private const char TrimChar = '\0';

    public async Task<CreatePersonResult> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var trimmedStackItems = (request.Stack ?? [])
                .Select(item => item.TrimEnd(TrimChar))
                .ToList();
        Person person = new()
        {
            Id = Guid.NewGuid(),
            Apelido = (request.Apelido).TrimEnd(TrimChar),
            Nome = (request.Nome).TrimEnd('\0'),
            Nascimento = request.Nascimento,
            Stack = trimmedStackItems,
        };
        person.SearchField = TermGenerator.BuildSearchField(person).ToString();
        await publisher.Publish(PersonCreatedEvent.From(person), cancellationToken);
        return CreatePersonResult.Success(person.Id);
    }

}
