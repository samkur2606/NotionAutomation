using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionDatabaseService(INotionClient notionClient)
{
    public INotionClient NotionClient { get; } = notionClient;

    public async Task<List<IWikiDatabase>> QueryDatabaseAsync(string notionDatabaseId, int pageSize)
    {
        var result = await NotionClient.Databases.QueryAsync(notionDatabaseId, new DatabasesQueryParameters { PageSize = pageSize });
        return result.Results;
    }

    //public async Task<IEnumerable<Holiday>> GetTodayHolidaysAsync()
    //{
    //    var today = DateTime.Today;
    //    var query = new DatabasesQueryParameters
    //    {
    //        Filter = new FilterObject
    //        {
    //            Property = "Date",
    //            Date = new DateFilter { Equals = today }
    //        }
    //    };

    //    var result = await NotionClient.Databases.QueryAsync("HOLIDAYS_DATABASE_ID", query);
    //    var parser = new NotionPropertyParser();

    //    var finalResult = result.Results.Select(page => new Holiday
    //    {
    //        Name = parser.GetNotionPageName(page),
    //        Date = parser.GetNotionPageDate(page, "Date")?.Date ?? DateTime.MinValue,
    //        IsBusinessDay = page.Properties.ContainsKey("Formula Is Business Day")
    //                        && page.Properties["Formula Is Business Day"] is CheckboxPropertyValue cb
    //            ? cb.Checkbox
    //            : false
    //    });

    //    return finalResult;
    //}

}