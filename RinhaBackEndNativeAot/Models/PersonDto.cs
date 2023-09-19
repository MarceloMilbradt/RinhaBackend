namespace RinhaBackEndNativeAot.Models;

public sealed record PersonDto(string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack);