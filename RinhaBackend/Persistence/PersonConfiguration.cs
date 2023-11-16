using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasIndex(p => p.Id);
        builder.Property(p => p.SearchField).HasComputedColumnSql("LOWER(\"Nome\" || \"Apelido\" || \"Stack\")");;
        builder.Property(p => p.Nascimento).HasColumnType("date");
        builder.Property(p => p.Stack).HasConversion(new StringArrayConverter()); ;
    }
}

public class StringArrayConverter : ValueConverter<string[], string>
{
    public StringArrayConverter() : base(
        v => string.Join(" ", v),
        v => v.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    { }
}