using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RinhaBackend.Models;

public class Person
{
    public Person()
    {
    }
    public Person(PersonDto personDto)
    {
        Id = personDto.Id;
        Apelido = personDto.Apelido;
        Nome = personDto.Nome;
        Nascimento = personDto.Nascimento;
        Stack = new List<string>(personDto.Stack ?? Array.Empty<string>());
    }

    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(32)]
    public string? Apelido { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Nome { get; set; }
    [Required]
    public DateOnly Nascimento { get; set; }
    public List<string> Stack { get; set; } = new List<string>();

    [JsonIgnore]
    public string SearchField { get; set; }
}

public record PersonDto(Guid Id, string? Apelido, string? Nome, DateOnly Nascimento, string[]? Stack);