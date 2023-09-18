using MediatR;

namespace RinhaBackend.Persons.Commands;

internal interface ICreatePersonRequestWithValidation : IRequest<CreatePersonResult>
{
    string? Apelido { get; init; }
    DateOnly Nascimento { get; init; }
    string? Nome { get; init; }
    string[]? Stack { get; init; }
}