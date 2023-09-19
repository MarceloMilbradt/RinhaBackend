using MemoryPack;

namespace RinhaBackEndNativeAot.Models;

[MemoryPackable(GenerateType.VersionTolerant)]
internal partial class Person
{
    [MemoryPackOrder(0)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MemoryPackOrder(1)]
    public string? Apelido { get; set; }
    [MemoryPackOrder(2)]
    public string? Nome { get; set; }
    [MemoryPackOrder(3)]
    public DateOnly Nascimento { get; set; }
    [MemoryPackOrder(4)]
    public List<string> Stack { get; set; } = [];
    [MemoryPackIgnore]
    public string? SearchField { get; set; } = string.Empty;
}
