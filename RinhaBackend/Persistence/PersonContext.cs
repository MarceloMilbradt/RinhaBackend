using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using System.Reflection;

namespace RinhaBackend.Persistence;

public class PersonContext : DbContext
{
    public PersonContext(DbContextOptions<PersonContext> options) : base(options)
    {
    }
    public DbSet<Person> Persons => Set<Person>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public static readonly Func<PersonContext, string, IAsyncEnumerable<Person>> SearchPersonsCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context, string term) =>
        context.Persons.Where(c => EF.Functions.Like(c.SearchField, $"%{term}%")).AsNoTracking().Take(50));

    public static readonly Func<PersonContext, Task<int>> CountPersonsCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context) => context.Persons.Count());
}


