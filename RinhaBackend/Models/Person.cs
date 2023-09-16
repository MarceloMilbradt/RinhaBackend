using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RinhaBackend.Models;

public class Person
{
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
    public string? SearchField { get; set; }
}