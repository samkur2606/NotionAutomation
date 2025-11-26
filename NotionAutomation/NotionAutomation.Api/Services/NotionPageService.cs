using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionPageService(INotionClient notionClient, NotionPageUpdateBuilder notionPageUpdateBuilder)
{
    private INotionClient NotionClient { get; } = notionClient;
    private NotionPageUpdateBuilder NotionPageUpdateBuilder { get; } = notionPageUpdateBuilder;

    public async Task<Page> UpdateTimesheetTypePropertyAsync(TimeSheet timesheet, TimeSheetType updateSheetType)
    {
        var property = NotionPageUpdateBuilder.CreateNotionPropertyUpdate(NotionNames.TimeSheets.Properties.Type, updateSheetType);
        var updateParams = NotionPageUpdateBuilder.CreatePagesUpdateParameters(property);
        var updatedPage = await NotionClient.Pages.UpdateAsync(timesheet.PageId.ToString(), updateParams);
        return updatedPage;
    }
}