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

}
