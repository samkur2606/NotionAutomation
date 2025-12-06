using System.ComponentModel;
using Notion.Client;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Scheduling;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

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

    public static WebApplicationBuilder AddCustomLoggingConfiguration(this WebApplicationBuilder builder, AppSettings appSettings)
    {
        var notionClient = builder.Services.BuildServiceProvider().GetRequiredService<INotionClient>();
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";
        var discordWebhookId = DiscordUrlExtractor.ExtractWebhookId(appSettings.DiscordWebhookUrl);
        var discordWebhookToken = DiscordUrlExtractor.ExtractWebhookToken(appSettings.DiscordWebhookUrl);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File("bin/Debug/net8.0/logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
            .WriteTo.Sink(new NotionSink(appSettings, notionClient))
            .WriteTo.Logger(i => i
                .MinimumLevel.Error()
                .WriteTo.Discord(discordWebhookId, discordWebhookToken)
            )
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }
}