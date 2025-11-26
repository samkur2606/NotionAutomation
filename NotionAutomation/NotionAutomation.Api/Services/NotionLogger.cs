using System.Diagnostics;
using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionLogger<T>(INotionClient notionClient, ConfigurationHelper configurationHelper, NotionPageUpdateBuilder notionPageUpdateBuilder) : ILogger<T>
{
    private INotionClient NotionClient { get; } = notionClient;
    private ConfigurationHelper ConfigurationHelper { get; } = configurationHelper;
    private NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = notionPageUpdateBuilder;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);

        if (exception != null)
            message += $"\nException: {exception}";

        _ = LogToNotionAsync(message, logLevel);
    }

    private async Task LogToNotionAsync(string message, LogLevel logLevel)
    {
        try
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";
            var databaseId = ConfigurationHelper.GetDatabaseId(NotionNames.NotionAutomationLogs.Database);
            var notionPropertyUpdate = new NotionPropertyUpdate { Type = NotionPropertyUpdateType.Title, Name = NotionNames.NotionAutomationLogs.Properties.Name, Value = logMessage };
            var pagesCreateParameters = NotionPageUpdateBuilder.CreatePagesCreateParameters(databaseId, notionPropertyUpdate);
            await NotionClient.Pages.CreateAsync(pagesCreateParameters);
        }
        catch
        {
            var errorMessage = "NotionLogger failed to log: " + message;
            Debug.WriteLine("NotionLogger failed to log: " + message);
        }
    }
}