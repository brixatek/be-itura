using System.Text.Json;

namespace Itura.Payment.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new
            {
                success = false,
                error = new { code = "Internal.Error", message = "An unexpected error occurred." }
            });
            await context.Response.WriteAsync(body);
        }
    }
}
