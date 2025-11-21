using Notion.Client;
using NotionAutomation.Api.Models;
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

    public static IServiceCollection AddCustomNotionClient(this IServiceCollection services)
    {
        services.AddSingleton<INotionClient>(sp =>
        {
            var appSettings = sp.GetRequiredService<AppSettings>();
            return NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = appSettings.Notion.IntegrationToken
            });
        });

        return services;
    }
}