using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionDatabaseService(INotionClient notionClient, ConfigurationHelper configurationHelper, NotionMapper notionMapper)
{
    private INotionClient NotionClient { get; } = notionClient;
    private ConfigurationHelper ConfigurationHelper { get; } = configurationHelper;
    private NotionMapper NotionMapper { get; } = notionMapper;

    public async Task<List<IWikiDatabase>> QueryDatabaseAsync(string notionDatabaseId, int pageSize)
    {
        var result = await NotionClient.Databases.QueryAsync(notionDatabaseId, new DatabasesQueryParameters { PageSize = pageSize });
        return result.Results;
    }

    public async Task<Holiday?> GetTodayHolidaysAsync(DateTime dateTime)
    {
        var dateFilter = new DateFilter("Date", dateTime);
        var queryParameters = new DatabasesQueryParameters { Filter = dateFilter };
        var databaseId = ConfigurationHelper.GetDatabaseId(NotionDatabaseNames.Holidays);

        var response = await NotionClient.Databases.QueryAsync(databaseId, queryParameters);

        var wikiDatabase = response.Results.FirstOrDefault();
        if (wikiDatabase is null)
            return null;

        var holiday = NotionMapper.MapToHoliday(wikiDatabase);
        return holiday;
    }
}