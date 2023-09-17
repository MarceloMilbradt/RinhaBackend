using System.Text;

namespace RinhaBackend.Filters;

public class RequestErrorCaptureMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException)
        {
            context.Response.StatusCode = 400;
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
        }
    }
}
