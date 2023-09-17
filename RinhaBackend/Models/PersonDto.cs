namespace RinhaBackend.Models;

public record PersonDto(Guid Id, string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack);