using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Id);
        builder.Property(p => p.SearchField);

        builder.Property(p => p.Nascimento).HasColumnType("date");
        builder.Property(p => p.Stack).HasColumnType("text[]");
    }
}
