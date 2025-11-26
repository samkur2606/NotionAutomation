using System.Diagnostics;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Services;

namespace NotionAutomation.Api.Logic;

public class TimeSheetManager(NotionDatabaseService notionDatabaseService, NotionPageService notionPageService, NotionMapper notionMapper, NotionRawApiService notionRawApiService, ILogger<TimeSheetManager> logger, INotificationService notificationService)
{
    private NotionDatabaseService NotionDatabaseService { get; } = notionDatabaseService;
    private NotionPageService NotionPageService { get; } = notionPageService;
    private NotionMapper NotionMapper { get; } = notionMapper;
    private NotionRawApiService NotionRawApiService { get; } = notionRawApiService;
    private ILogger<TimeSheetManager> Logger { get; } = logger;
    private INotificationService NotificationService { get; } = notificationService;

    public async Task UpdateTimesheetForTodayHolidayAsync()
    {
        var today = DateTime.Today;
        
        var holiday = await NotionDatabaseService.GetHolidaysByDateAsync(today);
        if (holiday is null) return;

        var timesheet = await NotionDatabaseService.GetTimesheetByDateAsync(today);
        if (timesheet is null) throw new Exception($"Timesheet not found for date {today:yyyy-MM-dd}. Cannot mark holiday.");

        var updatedPage = await NotionPageService.UpdateTimesheetTypePropertyAsync(timesheet, TimeSheetType.Holiday);
        var updatedTimeSheet = NotionMapper.MapToTimeSheet(updatedPage);
        
        if (updatedTimeSheet.Type == TimeSheetType.VacationDay)
        {
            var message = $"Timesheet for {today:yyyy-MM-dd} successfully updated to {nameof(TimeSheetType.Holiday)}.";
            Logger.LogInformation(message);
            await NotificationService.NotifySuccess(message);
        }
        else
        {
            var message = $"Failed to update timesheet for {today:yyyy-MM-dd} to {nameof(TimeSheetType.Holiday)}. Current type: {updatedTimeSheet.Type}";
            Logger.LogError(message);
            await NotificationService.NotifyError(message);
        }
    }

    public async Task UpdateTimesheetForTodayVacationAsync()
    {
        var today = DateTime.Today;
        
        var vacations = await NotionRawApiService.GetVacationsByDateAsync(today);
        if (!vacations.Any()) return;

        var timesheet = await NotionDatabaseService.GetTimesheetByDateAsync(today);
        if (timesheet is null) throw new Exception($"Timesheet not found for date {today:yyyy-MM-dd}. Cannot mark holiday.");

        var updatedPage = await NotionPageService.UpdateTimesheetTypePropertyAsync(timesheet, TimeSheetType.VacationDay);
        var updatedTimeSheet = NotionMapper.MapToTimeSheet(updatedPage);

        if (updatedTimeSheet.Type == TimeSheetType.VacationDay)
        {
            var message = $"Timesheet for {today:yyyy-MM-dd} successfully updated to {nameof(TimeSheetType.VacationDay)}.";
            Logger.LogInformation(message);
            await NotificationService.NotifySuccess(message);
        }
        else
        {
            var message = $"Failed to update timesheet for {today:yyyy-MM-dd} to {nameof(TimeSheetType.VacationDay)}. Current type: {updatedTimeSheet.Type}";
            Logger.LogError(message);
            await NotificationService.NotifyError(message);
        }
    }
}