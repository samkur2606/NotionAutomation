using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

    public async Task<string> GetAllRowsRaw(string databaseName)
    {
        var notionDatabase = AppSettings.Notion.Databases.FirstOrDefault(d => d.Name == databaseName);
        if (notionDatabase == null) throw new ArgumentException($"Database '{databaseName}' not found.");

        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabase.Id}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        return jsonString;
    }

    public async Task TestCall()
    {
        var response = await NotionClient.Databases.QueryAsync(AppSettings.Notion.Databases.First().Id, new DatabasesQueryParameters { PageSize = 100 });

        //var jsonFormat = await GetAllRowsRaw(AppSettings.Notion.Databases.First().Name);

        var pages = new List<NotionPage>();
        foreach (var result in response.Results)
        {
            var title = GetNotionPageName(result);
            var date = GetNotionPageDate(result, "MyDate");

            pages.Add(new NotionPage
            {
                Name = title,
                Date = date,
                Select = null,
                Created = created,
                Description = null
            });
        }
    }

    private string GetNotionPageName(IWikiDatabase wikiDatabase)
    {
        if (wikiDatabase is not Page page) throw new ArgumentNullException(nameof(page));

        if (page.Properties["Name"] is TitlePropertyValue titlePropertyValue) 
            return titlePropertyValue.Title.FirstOrDefault()?.PlainText ?? string.Empty;

        return string.Empty;
    }

    private DateTimeOffset? GetNotionPageDate(IWikiDatabase wikiDatabase, string propertyName)
    {
        if (wikiDatabase is not Page page)
            return null;

        if (page.Properties[propertyName] is DatePropertyValue { Date: not null } datePropertyValue)
            return datePropertyValue.Date.Start;

        return null;
    }
}