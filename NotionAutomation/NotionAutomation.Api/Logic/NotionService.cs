using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Client;
using NotionAutomation.Api.Models;
using static SQLite.SQLite3;
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

    public async Task<string> GetAllRowsRawAsync(string databaseName)
    {
        var notionDatabase = AppSettings.Notion.Databases.FirstOrDefault(d => d.Name == databaseName);
        if (notionDatabase == null) throw new ArgumentException($"Database '{databaseName}' not found.");

        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabase.Id}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        return jsonString;
    }

    public async Task<List<NotionPage>> GetNotionPagesAsync(string databaseName, int pageSize)
    {
        var notionDatabase = AppSettings.Notion.Databases.FirstOrDefault(d => d.Name == databaseName);
        if (notionDatabase == null) throw new ArgumentException($"Database '{databaseName}' not found.");
        var response = await NotionClient.Databases.QueryAsync(notionDatabase.Id, new DatabasesQueryParameters { PageSize = pageSize });
        var result = response.Results
            .Select(MapToNotionPage)
            .ToList();
        return result;
    }

    public NotionPage MapToNotionPage(IWikiDatabase wikiDatabase)
    {
        var title = GetNotionPageName(wikiDatabase);
        var date = GetNotionPageDate(wikiDatabase, "MyDate");
        var created = GetNotionPageCreatedTime(wikiDatabase);
        var select = GetNotionPageSelect(wikiDatabase, "MySelect");
        var description = GetNotionPageRichText(wikiDatabase, "MyDescription");
        var id = GetNotionPageId(wikiDatabase);

        var notionPage = new NotionPage
        {
            Id = id,
            Name = title,
            Date = date,
            Select = select,
            Created = created,
            Description = description
        };

        return notionPage;
    }

    public async Task TestCall()
    {
        var databaseName = AppSettings.Notion.Databases.First().Name;
        var pages = await GetNotionPagesAsync(databaseName, 1);
        var page = pages[0];

        var properties = new Dictionary<string, object>
        {
            ["MySelect"] = new SelectOption { Name = "Test2" },
            ["MyDate"] = DateTimeOffset.Now
        };

        var pagesUpdateParameters = CreatePagesUpdateParameters(properties);
        var updatedPage = await UpdateNotionPageAsync(page.Id, pagesUpdateParameters);
    }

    public async Task<NotionPage> UpdateNotionPageAsync(Guid notionPageId, PagesUpdateParameters parameters)
    {
        var updatedPage = await NotionClient.Pages.UpdateAsync(notionPageId.ToString(), parameters);
        if (updatedPage is null) throw new Exception("Update failed: Notion returned null.");
        var notionPage = MapToNotionPage(updatedPage);
        return notionPage;
    }

    private static PagesUpdateParameters CreatePagesUpdateParameters(Dictionary<string, object> properties)
    {

        var notionProperties = new Dictionary<string, PropertyValue>();
        foreach (var (propertyName, propertyValue) in properties)
        {
            notionProperties[propertyName] = propertyValue switch
            {
                SelectOption selectOption => new SelectPropertyValue { Select = selectOption },
                DateTimeOffset dateTimeOffset => new DatePropertyValue { Date = new Date { Start = dateTimeOffset } },
                _ => throw new ArgumentException($"Unsupported type for '{propertyName}'.")
            };
        }

        var pagesUpdateParameters = new PagesUpdateParameters { Properties = notionProperties };
        return pagesUpdateParameters;
    }

    private Guid GetNotionPageId(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        var id = Guid.Parse(page.Id);
        return id;
    }

    private string GetNotionPageRichText(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);

        if (page.Properties[propertyName] is RichTextPropertyValue { RichText: not null } richTextPropertyValue)
            return string.Join(" ", richTextPropertyValue.RichText.Select(rt => rt.PlainText));

        return string.Empty;
    }

    private string GetNotionPageSelect(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);

        if (page.Properties[propertyName] is SelectPropertyValue { Select: not null } selectPropertyValue) return selectPropertyValue.Select.Name;

        return string.Empty;
    }

    private DateTimeOffset GetNotionPageCreatedTime(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        return page.CreatedTime;
    }

    private string GetNotionPageName(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        if (page.Properties["Name"] is TitlePropertyValue titlePropertyValue)
            return titlePropertyValue.Title.FirstOrDefault()?.PlainText ?? string.Empty;

        return string.Empty;
    }

    private DateTimeOffset? GetNotionPageDate(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        if (page.Properties[propertyName] is DatePropertyValue { Date: not null } datePropertyValue)
            return datePropertyValue.Date.Start;

        return null;
    }

    private Page GetPageOrThrowException(IWikiDatabase wikiDatabase)
    {
        if (wikiDatabase is not Page page)
            throw new ArgumentException("The provided object is not a Page.", nameof(wikiDatabase));
        return page;
    }
}