using System.Diagnostics;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Services;

namespace NotionAutomation.Api.Logic;

public class TimeSheetManager(NotionDatabaseService notionDatabaseService, NotionPageService notionPageService, NotionMapper notionMapper)
{
    private NotionDatabaseService NotionDatabaseService { get; } = notionDatabaseService;
    private NotionPageService NotionPageService { get; } = notionPageService;
    private NotionMapper NotionMapper { get; } = notionMapper;

    public async Task UpdateTimesheetForTodayHolidayAsync()
    {
        var today = DateTime.Today;
        
        var holiday = await NotionDatabaseService.GetHolidaysByDateAsync(today);
        if (holiday is null) return;

        var timesheet = await NotionDatabaseService.GetTimesheetByDateAsync(today);
        if (timesheet is null) throw new Exception($"Timesheet not found for date {today:yyyy-MM-dd}. Cannot mark holiday.");

        var updatedPage = await NotionPageService.UpdateTimesheetTypePropertyAsync(timesheet, TimeSheetType.Holiday);
        var updatedTimeSheet = NotionMapper.MapToTimeSheet(updatedPage);
        // todo(sk): introducing logging
        Debug.WriteLine(updatedTimeSheet.Type == TimeSheetType.Holiday
            ? $"Timesheet for {today:yyyy-MM-dd} successfully updated to Holiday."
            : $"Failed to update timesheet for {today:yyyy-MM-dd} to Holiday. Current type: {updatedTimeSheet.Type}");
    }
}