using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;
using System.Reflection;
using System.Reflection.Emit;

namespace RinhaBackend.Persistence;

public class PersonContext : DbContext, IPersonDbContext
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


    public static readonly Func<PersonContext, string, Task<List<Person>>> SearchPersonsCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context, string term) => context.Persons.Where(c => c.SearchField.Contains(term)).ToList());


    public static readonly Func<PersonContext, Guid, Task<Person?>> FindPersonByIdCompiledQueryAsync =
        EF.CompileAsyncQuery((PersonContext context, Guid id) => context.Persons.Find(id));

    public static readonly Func<PersonContext, Task<int>> CountPersonsCompiledQueryAsync =
    EF.CompileAsyncQuery((PersonContext context) => context.Persons.Count());


}
