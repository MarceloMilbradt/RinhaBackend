using RinhaBackend.Persistence;

namespace Microsoft.Extensions.DependencyInjection;

public static class PersonDbContextExtensions
{
    public static IServiceCollection AddPersonContext(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddTransient<IPersonRepository, PersonDapperRepository>();
        return services;
    }
}
