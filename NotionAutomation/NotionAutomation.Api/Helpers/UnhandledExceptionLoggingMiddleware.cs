namespace NotionAutomation.Api.Helpers;

public class UnhandledExceptionLoggingMiddleware(RequestDelegate next, ILogger<UnhandledExceptionLoggingMiddleware> logger)
{
    private RequestDelegate Next { get; } = next;
    private ILogger<UnhandledExceptionLoggingMiddleware> Logger { get; } = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            throw;
        }
    }
}