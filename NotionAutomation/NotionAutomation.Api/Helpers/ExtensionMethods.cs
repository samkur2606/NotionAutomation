using NotionAutomation.Api.Scheduling;

namespace NotionAutomation.Api.Helpers;

public static class ExtensionMethods
{
    public static void UseHangfireUiInEnglish(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<HangfireUiInEnglishMiddleware>();
    }

    public static void RegisterRecurringJobs(this IApplicationBuilder builder)
    {
        JobScheduler.RegisterJobs();
    }
}