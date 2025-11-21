using System.Net.Http.Headers;
using System.Text;
using Notion.Client;
using NotionAutomation.Api.Models;
using Page = Notion.Client.Page;

namespace NotionAutomation.Api.Logic;

public class NotionService
{
    public NotionService(AppSettings appSettings)
    {
        AppSettings = appSettings;

        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);

        NotionClient = NotionClientFactory.Create(new ClientOptions { AuthToken = appSettings.Notion.IntegrationToken });
    }

    private INotionClient NotionClient { get; }
    private AppSettings AppSettings { get; }
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

    public async Task TestCallOld()
    {
        var notionDatabaseName = AppSettings.Notion.Databases.First().Name;
        var rows = await GetAllRowsAsync(notionDatabaseName);
    }

    public async Task TestCall()
    {
        var queryResult = await NotionClient.Databases.QueryAsync(AppSettings.Notion.Databases.First().Id, new DatabasesQueryParameters { PageSize = 100 });

        var pages = new List<NotionPage>();
        foreach (var result in queryResult.Results)
        {
            var title = GetNotionPageName(result);

            pages.Add(new NotionPage
            {
                Name = title,
                Date = null,
                Select = null,
                Created = DateTime.Now,
                Description = null
            });
        }
    }

    private string GetNotionPageName(IWikiDatabase wikiDatabase)
    {
        if (wikiDatabase is not Page page) throw new ArgumentNullException(nameof(page));

        if (page.Properties["Name"] is TitlePropertyValue titleProp) 
            return titleProp.Title.FirstOrDefault()?.PlainText ?? string.Empty;

        return string.Empty;
    }
}