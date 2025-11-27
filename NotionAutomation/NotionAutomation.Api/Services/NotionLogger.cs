using System.Diagnostics;
using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionLogger<T>(INotionClient notionClient, AppSettings appSettings, ILoggerFactory loggerFactory)
    : ILogger<T>
{
    private INotionClient NotionClient { get; } = notionClient;
    private AppSettings AppSettings { get; } = appSettings;
    private NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = new();
    public ILogger<T> InnerLogger { get; set; } = loggerFactory.CreateLogger<T>();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => InnerLogger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => InnerLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!InnerLogger.IsEnabled(logLevel)) return;


        var message = formatter(state, exception);

        if (exception != null)
            message += $"\nException: {exception}";

        _ = LogToNotionAsync(message, logLevel);
    }

    private async Task LogToNotionAsync(string message, LogLevel logLevel)
    {
        try
        {
            var databaseId = AppSettings.Notion.Databases.First(i => i.Name == NotionNames.NotionAutomationLogs.Database).Id;
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";
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