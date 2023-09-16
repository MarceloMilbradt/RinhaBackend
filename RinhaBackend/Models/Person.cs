using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RinhaBackend.Models;


public class Person
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(32)]
    public string Apelido { get; set; }
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }
    [Required]
    public DateTime Nascimento { get; set; }
    public List<string> Stack { get; set; } = new List<string>();
    public string SearchField { get; set; }
}