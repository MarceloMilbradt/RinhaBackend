using MemoryPack;
using System.Text.Json.Serialization;

namespace RinhaBackend.Application;

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Person
{
    [MemoryPackOrder(0)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MemoryPackOrder(1)]
    public string? Apelido { get; set; }
    [MemoryPackOrder(2)]
    public string? Nome { get; set; }
    [MemoryPackOrder(3)]
    public DateTime Nascimento { get; set; }
    [MemoryPackOrder(4)]
    public string[] Stack { get; set; } = [];
}
