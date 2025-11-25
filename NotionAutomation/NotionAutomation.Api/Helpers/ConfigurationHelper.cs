using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Helpers;

public class ConfigurationHelper(AppSettings appSettings)
{
    private AppSettings AppSettings { get; } = appSettings;

    public string GetDatabaseId(string name)
    {
        var databases = AppSettings.Notion.Databases.ToDictionary(d => d.Name, d => d.Id);

        if (databases.TryGetValue(name, out var id))
            return id;

        throw new KeyNotFoundException($"DatabaseName '{name}' not found in configuration.");
    }
}