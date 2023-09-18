using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RinhaBackend.Models;

internal sealed class Person
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string? Apelido { get; set; }
    [Required]
    [MaxLength(150)]
    public string? Nome { get; set; }
    [Required]
    public DateOnly Nascimento { get; set; }
    public List<string> Stack { get; set; } = new List<string>();

    [JsonIgnore]
    [MaxLength(500)]
    public string? SearchField { get; set; } = string.Empty;
}
