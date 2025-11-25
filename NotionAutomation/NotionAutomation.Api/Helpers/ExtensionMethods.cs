using System.ComponentModel;
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

    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        if (memberInfo.Length <= 0) return value.ToString();
        var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        var description = attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : value.ToString();
        return description;
    }
}