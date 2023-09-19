using MemoryPack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RinhaBackend.Models;
[MemoryPackable(GenerateType.VersionTolerant)]
internal partial class Person
{
    [Key]
    [MemoryPackOrder(0)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    [MemoryPackOrder(1)]
    public string? Apelido { get; set; }
    [Required]
    [MaxLength(150)]
    [MemoryPackOrder(2)]
    public string? Nome { get; set; }
    [Required]
    [MemoryPackOrder(3)]
    public DateOnly Nascimento { get; set; }
    [MemoryPackOrder(4)]
    public List<string> Stack { get; set; } = new List<string>();

    [JsonIgnore]
    [MaxLength(500)]
    [MemoryPackIgnore]
    public string? SearchField { get; set; } = string.Empty;
}
