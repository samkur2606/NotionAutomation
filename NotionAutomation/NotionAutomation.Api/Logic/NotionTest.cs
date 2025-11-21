using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Services;
using static SQLite.SQLite3;
using Page = Notion.Client.Page;

namespace NotionAutomation.Api.Logic;

public class NotionTest(
    NotionRawApiService notionRawApiService,
    NotionDatabaseService notionDatabaseService,
    NotionPageService notionPageService,
    AppSettings appSettings,
    NotionPageUpdateBuilder notionPageUpdateBuilder)
{
    public NotionRawApiService NotionRawApiService { get; } = notionRawApiService;
    public NotionDatabaseService NotionDatabaseService { get; } = notionDatabaseService;
    public NotionPageService NotionPageService { get; } = notionPageService;
    public AppSettings AppSettings { get; } = appSettings;
    public NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = notionPageUpdateBuilder;

    public async Task TestCall()
    {
        var database = AppSettings.Notion.Databases.First();

        var rawData = await NotionRawApiService.QueryDatabaseRawAsync(database.Id);
        var pages = await NotionDatabaseService.QueryDatabaseAsync(database.Id, 1);

        var page = pages[0];
        var properties = new Dictionary<string, object>
        {
            ["MySelect"] = new SelectOption { Name = "Test2" },
            ["MyDate"] = DateTimeOffset.Now
        };

        var pagesUpdateParameters = NotionPageUpdateBuilder.CreatePagesUpdateParameters(properties);
        var updatedPage = await NotionPageService.UpdatePageAsync(page.Id, pagesUpdateParameters);
    }
}



