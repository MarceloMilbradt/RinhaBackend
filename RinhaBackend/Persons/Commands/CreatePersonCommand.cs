using MediatR;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;

namespace RinhaBackend.Persons.Commands;
internal sealed record CreatePersonCommand(string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack) : IRequest<Guid>;

internal sealed class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly IPublisher _publisher;
    private readonly PersonContext _personContext;
    public CreatePersonCommandHandler(IPublisher publisher, PersonContext personContext)
    {
        _publisher = publisher;
        _personContext = personContext;
    }

    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {

        Person newPerson = new()
        {
            Id = Guid.NewGuid(),
            Apelido = request.Apelido,
            Nome = request.Nome,
            Nascimento = request.Nascimento,
            Stack = new List<string>(request.Stack ?? Array.Empty<string>()),
        };
        newPerson.BuildSearchField();
        await _publisher.Publish(PersonCreatedEvent.From(newPerson), cancellationToken);
        return newPerson.Id;
    }
}