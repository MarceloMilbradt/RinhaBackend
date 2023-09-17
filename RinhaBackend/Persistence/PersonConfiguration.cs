using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Id);
        builder.HasIndex(p => p.Apelido).IsUnique();
        // Use the new custom function for the computed column
        builder.Property(p => p.SearchField)
               .HasComputedColumnSql("""generate_search_field("Apelido", "Nome", "Nascimento", "Stack")""", stored: true);

        builder.Property(p => p.Nascimento).HasColumnType("date");
        builder.Property(p => p.Stack).HasColumnType("text[]");
        builder.HasIndex(p => p.SearchField);
    }
}
