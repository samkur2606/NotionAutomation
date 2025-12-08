using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        var truncatedMessage = TruncateMessage(logEvent.RenderMessage());
        var truncatedExceptionMessage = TruncateMessage(logEvent.Exception?.ToString());
        var truncatedJson = TruncateMessage(GetJson(logEvent));

        var notionLog = new NotionLog
        {
            Title = CreateTitleHash(logEvent),
            LogLevel = Enum.Parse<NotionLogLevel>(logEvent.Level.ToString()),
            Message = truncatedMessage,
            Exception = truncatedExceptionMessage,
            Source = source,
            AdditionalData = truncatedJson,
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
        var jsonDict = new Dictionary<string, object?>
        {
            ["Timestamp"] = logEvent.Timestamp,
            ["Level"] = logEvent.Level.ToString(),
            ["Message"] = logEvent.RenderMessage(),
            ["SourceContext"] = logEvent.Properties.TryGetValue("SourceContext", out var sourceContext) ? ConvertLogEventPropertyValue(sourceContext) : null,
            ["Exception"] = TruncateMessage(logEvent.Exception?.ToString(), 100),
            ["Properties"] = logEvent.Properties.ToDictionary(
                kvp => kvp.Key,
                kvp => ConvertLogEventPropertyValue(kvp.Value)
            )
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(jsonDict, options);
        return json;
    }

    private object? ConvertLogEventPropertyValue(LogEventPropertyValue value)
    {
        return value switch
        {
            ScalarValue scalar => scalar.Value,
            SequenceValue seq => seq.Elements.Select(ConvertLogEventPropertyValue).ToArray(),
            StructureValue structVal => structVal.Properties.ToDictionary(p => p.Name, p => ConvertLogEventPropertyValue(p.Value)),
            DictionaryValue dictVal => dictVal.Elements.ToDictionary(
                e => e.Key.Value?.ToString() ?? "",
                e => ConvertLogEventPropertyValue(e.Value)
            ),
            _ => value.ToString()
        };
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

    private static string? TruncateMessage(string? message, int maxLength = 1800)
    {
        if (string.IsNullOrWhiteSpace(message)) return null;
        var truncatedMessage = message.Length <= maxLength ? message : message[..maxLength] + "... [truncated]";
        return truncatedMessage;
    }
}