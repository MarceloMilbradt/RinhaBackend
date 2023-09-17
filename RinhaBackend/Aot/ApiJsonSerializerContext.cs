using RinhaBackend.Models;
using System.Text.Json.Serialization;

namespace RinhaBackend.Aot;
[JsonSerializable(typeof(Person))]
public partial class ApiJsonSerializerContext : JsonSerializerContext
{
}
