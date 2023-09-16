using Microsoft.EntityFrameworkCore;
using RinhaBackend.Models;

namespace RinhaBackend.Persistence
{
    internal interface IPersonDbContext
    {
        DbSet<Person> Persons { get;}
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}