using System.Diagnostics;
using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using Serilog.Core;
using Serilog.Events;

namespace NotionAutomation.Api.Helpers;

public class NotionSink(AppSettings appSettings, INotionClient notionClient, IFormatProvider? formatProvider = null) : ILogEventSink
{
    private AppSettings AppSettings { get; } = appSettings;
    private INotionClient NotionClient { get; } = notionClient;
    private NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = new();


    public void Emit(LogEvent? logEvent)
    {
        if (logEvent == null) return;
        var message = logEvent.RenderMessage(formatProvider);
        _ = LogToNotionAsync(message);
    }

    private async Task LogToNotionAsync(string message)
    {
        try
        {
            var databaseId = AppSettings.Notion.Databases.First(i => i.Name == NotionNames.NotionAutomationLogs.Database).Id;
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
            var notionPropertyUpdate = new NotionPropertyUpdate
                { Type = NotionPropertyUpdateType.Title, Name = NotionNames.NotionAutomationLogs.Properties.Name, Value = logMessage };
            var pagesCreateParameters = NotionPageUpdateBuilder.CreatePagesCreateParameters(databaseId, notionPropertyUpdate);
            await NotionClient.Pages.CreateAsync(pagesCreateParameters);
        }
        catch
        {
            Debug.WriteLine("NotionLogger failed to log: " + message);
        }
    }
}