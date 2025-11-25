using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;
using NotionSchema = NotionAutomation.Api.Models.NotionSchema;

namespace NotionAutomation.Api.Services;

public class NotionDatabaseService(INotionClient notionClient, ConfigurationHelper configurationHelper, NotionMapper notionMapper)
{
    private INotionClient NotionClient { get; } = notionClient;
    private ConfigurationHelper ConfigurationHelper { get; } = configurationHelper;
    private NotionMapper NotionMapper { get; } = notionMapper;

    public async Task<List<IWikiDatabase>> QueryDatabaseAsync(Guid notionDatabaseId, int pageSize)
    {
        var result = await NotionClient.Databases.QueryAsync(notionDatabaseId.ToString(), new DatabasesQueryParameters { PageSize = pageSize });
        return result.Results;
    }

    public async Task<DatabaseQueryResponse> GetDataByDateAsync(Guid databaseId, DateTime dateTime, string notionDatePropertyName)
    {
        var dateFilter = new DateFilter(notionDatePropertyName, dateTime);
        var query = new DatabasesQueryParameters { Filter = dateFilter };
        var response = await NotionClient.Databases.QueryAsync(databaseId.ToString(), query);
        return response;
    }

    public async Task<Holiday?> GetHolidaysByDateAsync(DateTime dateTime)
    {
        var databaseId = ConfigurationHelper.GetDatabaseId(NotionSchema.Holidays.DatabaseName);
        var response = await GetDataByDateAsync(databaseId, dateTime, NotionSchema.Holidays.Properties.DateName);

        var wikiDatabase = response.Results.FirstOrDefault();
        if (wikiDatabase is null)
            return null;

        var holiday = NotionMapper.MapToHoliday(wikiDatabase);
        return holiday;
    }

    public async Task<TimeSheet?> GetTimesheetByDateAsync(DateTime dateTime)
    {
        var databaseId = ConfigurationHelper.GetDatabaseId(NotionSchema.TimeSheets.DatabaseName);
        var response = await GetDataByDateAsync(databaseId, dateTime, NotionSchema.TimeSheets.Properties.DateName);

        var wikiDatabase = response.Results.FirstOrDefault();
        if (wikiDatabase is null)
            return null;

        var timeSheet = NotionMapper.MapToTimeSheet(wikiDatabase);
        return timeSheet;
    }
}