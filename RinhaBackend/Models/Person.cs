using MemoryPack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RinhaBackend.Models;

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Person
{
    [MemoryPackOrder(0)]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MemoryPackOrder(1)]
    [MaxLength(40)]
    public string? Apelido { get; set; }
    [MemoryPackOrder(2)]
    [MaxLength(120)]
    public string? Nome { get; set; }
    [MemoryPackOrder(3)]
    public DateTime Nascimento { get; set; }
    [MemoryPackOrder(4)]
    [MaxLength(1024)]
    public string[] Stack { get; set; }
    [MemoryPackIgnore]
    [JsonIgnore]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string SearchField { get; set; }

}
