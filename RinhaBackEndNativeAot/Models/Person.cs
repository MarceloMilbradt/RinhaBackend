using MemoryPack;

namespace RinhaBackEndNativeAot.Models;

[MemoryPackable]
internal partial class Person
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Apelido { get; set; }
    public string? Nome { get; set; }
    public DateOnly Nascimento { get; set; }
    public List<string> Stack { get; set; } = [];
    [MemoryPackIgnore]
    public string? SearchField { get; set; } = string.Empty;
}
