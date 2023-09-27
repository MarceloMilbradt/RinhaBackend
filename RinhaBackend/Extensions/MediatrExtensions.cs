
namespace Microsoft.Extensions.DependencyInjection;

public static class MediatrExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediator();
        return services;
    }
}
