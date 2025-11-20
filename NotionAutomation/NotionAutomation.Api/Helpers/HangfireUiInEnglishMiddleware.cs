using System.Globalization;

namespace NotionAutomation.Api.Helpers;

public class HangfireUiInEnglishMiddleware(RequestDelegate next)
{
    // Middleware is discovered and invoked automatically by the ASP.NET pipeline.
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/hangfire"))
        {
            var en = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = en;
            Thread.CurrentThread.CurrentUICulture = en;

            CultureInfo.CurrentCulture = en;
            CultureInfo.CurrentUICulture = en;
        }

        await next(context);
    }
}