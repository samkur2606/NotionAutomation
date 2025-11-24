using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Services;

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

        
        var pages = await NotionDatabaseService.QueryDatabaseAsync(database.Id, 1);

        
    }
}