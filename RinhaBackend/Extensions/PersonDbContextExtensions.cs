using Microsoft.EntityFrameworkCore;
using RinhaBackend.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class PersonDbContextExtensions
{
    public static IServiceCollection AddPersonContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PersonContext>(options =>
            options.UseNpgsql(connectionString,
            builder =>
            {
                builder.CommandTimeout(60);

                builder.MigrationsAssembly(typeof(PersonContext).Assembly.FullName);
                builder.EnableRetryOnFailure();
            }));
        return services;
    }
}
