using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence;

public class PersonContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=db;Database=rinhabackend;Username=rinhabackend;Password=rinhabackend");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasIndex(p => p.Id); 

        modelBuilder.Entity<Person>()
            .Property(p => p.SearchField)
            .HasComputedColumnSql("[Apelido] + ' ' + [Nome] + ' ' + STRING_AGG([Stacks], ' ')");
        
        modelBuilder.Entity<Person>()
            .HasIndex(p => p.SearchField);
    }

}