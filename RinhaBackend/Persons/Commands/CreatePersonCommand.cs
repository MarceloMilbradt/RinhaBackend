using Mediator;
using RinhaBackend.Cache;
using RinhaBackend.Models;
using RinhaBackend.Persistence;
using RinhaBackend.Persons.Events;

namespace RinhaBackend.Persons.Commands;
public sealed record CreatePersonCommand(string? Apelido, string? Nome, DateTime Nascimento, string[]? Stack) : IRequest<CreatePersonResult>;
public sealed class CreatePersonCommandHandler(IPublisher publisher, IPersonRepository repository) : IRequestHandler<CreatePersonCommand, CreatePersonResult>
{
    public async ValueTask<CreatePersonResult> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(request.Apelido))
            return CreatePersonResult.Fail;

        if (string.IsNullOrWhiteSpace(request.Nome))
            return CreatePersonResult.Fail;

        if (request.Stack != null && request.Stack.Any(s => string.IsNullOrWhiteSpace(s)))
            return CreatePersonResult.Fail;

        Person person = new()
        {
            Id = Guid.NewGuid(),
            Apelido = request.Apelido,
            Nome = request.Nome,
            Nascimento = request.Nascimento,
            Stack = request.Stack ?? [],
        };
        await repository.Create(person, cancellationToken);
        await publisher.Publish(PersonCreatedEvent.From(person), cancellationToken);
        return CreatePersonResult.Success(person.Id);
    }
}
