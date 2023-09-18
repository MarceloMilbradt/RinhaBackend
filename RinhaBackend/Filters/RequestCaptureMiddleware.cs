using System.Text;

namespace RinhaBackend.Filters;

internal sealed class RequestErrorCaptureMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 400;
        }
    }
}
