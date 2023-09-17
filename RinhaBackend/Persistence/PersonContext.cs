using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using System.Reflection;
using System.Reflection.Emit;

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
        context.Persons.Where(c => c.SearchField!.Contains(term)));

    public static readonly Func<PersonContext, Task<int>> CountPersonsCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context) => context.Persons.Count());

    public static readonly Func<PersonContext, string, Task<bool>> AnyPersonsWithNicknameCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context, string apelido) => context.Persons.Any(p => p.Apelido == apelido));


}
