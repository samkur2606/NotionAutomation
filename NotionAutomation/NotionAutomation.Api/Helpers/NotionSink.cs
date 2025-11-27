using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

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
            var notionLog = CreateNotionLog(logEvent);
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

    private NotionLog CreateNotionLog(LogEvent logEvent)
    {
        var source = GetLogSource(logEvent);

        var notionLog = new NotionLog
        {
            Title = CreateTitleHash(logEvent),
            LogLevel = Enum.Parse<NotionLogLevel>(logEvent.Level.ToString()),
            Message = logEvent.RenderMessage(),
            Exception = logEvent.Exception?.ToString(),
            Source = source,
            AdditionalData = GetJson(logEvent),
            LoggingTime = logEvent.Timestamp
        };

        return notionLog;
    }

    private string GetLogSource(LogEvent logEvent)
    {
        var source = logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
            ? sourceContext.ToString().Trim('"')
            : string.Empty;
        return source;
    }

    private string CreateTitleHash(LogEvent logEvent)
    {
        var template = logEvent.MessageTemplate.Text;
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(template);
        var hashBytes = md5.ComputeHash(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "")[..8];
        return $"Log-{hash}";
    }

    private string GetJson(LogEvent logEvent)
    {
        var sw = new StringWriter();
        var formatter = new JsonFormatter(renderMessage: true);
        formatter.Format(logEvent, sw);
        return sw.ToString();
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
            new()
            {
                Type = NotionPropertyUpdateType.RichText, Name = NotionNames.NotionAutomationLogs.Properties.AdditionalData, Value = notionLog.AdditionalData!
            },
            new() { Type = NotionPropertyUpdateType.Date, Name = NotionNames.NotionAutomationLogs.Properties.LoggingTime, Value = notionLog.LoggingTime! }
        };
        return updateProperties;
    }
}