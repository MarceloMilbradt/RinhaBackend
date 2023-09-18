using Microsoft.EntityFrameworkCore;

namespace RinhaBackend.Persistence;

internal static class MigrationExtensions
{
    public static void InitializeDatabase(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var dbcontext = scope.ServiceProvider.GetRequiredService<PersonContext>();
        try
        {
            dbcontext.Database.Migrate();
            //Carrega as pessoas em memoria
            dbcontext.Persons.ExecuteDelete();
            dbcontext.Persons.Load();

        }
        catch (Exception) { }
    }
}
