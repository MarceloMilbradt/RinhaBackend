using MediatR;
using RinhaBackend.Pipeline;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class MediatrExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            options.NotificationPublisher = new FireAndForgetNotificationPublisher();
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PersonValidationBehavior<,>));
        });
        return services;
    }
}
