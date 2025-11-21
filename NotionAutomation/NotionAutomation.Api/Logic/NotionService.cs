using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Logic;

public class NotionService
{
    public NotionService(AppSettings appSettings)
    {
        AppSettings = appSettings;

        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);
    }

    public AppSettings AppSettings { get; }
    private HttpClient HttpClient { get; }

    public async Task<string> GetAllRowsAsync(string databaseName)
    {
        var notionDatabase = AppSettings.Notion.Databases.FirstOrDefault(d => d.Name == databaseName);
        if (notionDatabase == null) throw new ArgumentException($"Database '{databaseName}' not found.");

        var dataSourceId = notionDatabase.DataSourceId;
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{dataSourceId}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        return jsonString;
    }

    public async Task TestCall()
    {
        var notionDatabaseName = AppSettings.Notion.Databases.First().Name;
        var rows = await GetAllRowsAsync(notionDatabaseName);
    }
}