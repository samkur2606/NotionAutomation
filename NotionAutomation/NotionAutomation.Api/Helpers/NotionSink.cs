using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
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
    public IFormatProvider? FormatProvider { get; } = formatProvider;
    private NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = new();

    public void Emit(LogEvent? logEvent)
    {
        if (logEvent == null) return;
        
        _ = LogToNotionAsync(logEvent);
    }

    private async Task LogToNotionAsync(LogEvent logEvent)
    {
        try
        {
            var notionLog = new NotionLog
            {
                Title = "Title-To-Be-Done",
                LogLevel = Enum.Parse<NotionLogLevel>(logEvent.Level.ToString()),
                Message = logEvent.RenderMessage(),
                Exception = logEvent.Exception?.ToString(),
                Source = "Source-To-Be-Done",
                AdditionalData = "AdditionalData-To-Be-Done",
                LoggingTime = logEvent.Timestamp
            };

            var databaseId = AppSettings.Notion.Databases.First(i => i.Name == NotionNames.NotionAutomationLogs.Database).Id;
            var updateProperties = CreateNotionPropertyUpdate(notionLog);
            var pagesCreateParameters = NotionPageUpdateBuilder.CreatePagesCreateParameters(databaseId, updateProperties.ToArray());
            await NotionClient.Pages.CreateAsync(pagesCreateParameters);
        }
        catch
        {
            Debug.WriteLine("NotionLogger failed to log: " + logEvent.RenderMessage());
        }
    }

    private static List<NotionPropertyUpdate> CreateNotionPropertyUpdate(NotionLog notionLog)
    {
        var updateProperties = new List<NotionPropertyUpdate>
        {
            new() { Type = NotionPropertyUpdateType.Title, Name = NotionNames.NotionAutomationLogs.Properties.Name, Value = notionLog.Title! },
            new() { Type = NotionPropertyUpdateType.Select, Name = NotionNames.NotionAutomationLogs.Properties.LogLevel, Value = notionLog.LogLevel! },
            new() { Type = NotionPropertyUpdateType.RichText, Name = NotionNames.NotionAutomationLogs.Properties.Message, Value = notionLog.Message! },
            new() { Type = NotionPropertyUpdateType.RichText, Name = NotionNames.NotionAutomationLogs.Properties.Exception, Value = notionLog.Exception! },
            new() { Type = NotionPropertyUpdateType.RichText, Name = NotionNames.NotionAutomationLogs.Properties.Source, Value = notionLog.Source! },
            new() { Type = NotionPropertyUpdateType.RichText, Name = NotionNames.NotionAutomationLogs.Properties.AdditionalData, Value = notionLog.AdditionalData! },
            new() { Type = NotionPropertyUpdateType.Date, Name = NotionNames.NotionAutomationLogs.Properties.LoggingTime, Value = notionLog.LoggingTime! }
        };
        return updateProperties;
    }
}