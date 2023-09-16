using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Id);
        builder.Property(p => p.SearchField)
               .HasComputedColumnSql(@"""Apelido"" || ' ' || ""Nome"" || ' ' || array_to_string_immutable(""Stack"", ' ')", stored: true);
        builder.Property(p => p.Nascimento).HasColumnType("date");

        builder.HasIndex(p => p.SearchField);
    }
}
