using Microsoft.EntityFrameworkCore;

namespace RinhaBackend.Persistence;

public static class MigrationExtensions
{
    public static void InitializeDatabase(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<PersonContext>();
        dbcontext.Database.Migrate();
        dbcontext.Persons.Load();
    }
}
