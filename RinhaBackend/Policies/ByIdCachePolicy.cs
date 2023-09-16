using Microsoft.AspNetCore.OutputCaching;

namespace RinhaBackend.Policies;

public class ByIdCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var idRoute = context.HttpContext.Request.RouteValues["id"];
        if (idRoute == null)
        {
            return ValueTask.CompletedTask;
        }
        context.Tags.Add(idRoute.ToString()!);
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation) => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation) => ValueTask.CompletedTask;
}