using NotionAutomation.Api.Services;

namespace NotionAutomation.Api.Logic;

public class VacationManager(NotionDatabaseService notionDatabaseService, NotionRawApiService notionRawApiService)
{
    private NotionDatabaseService NotionDatabaseService { get; } = notionDatabaseService;
    private NotionRawApiService NotionRawApiService { get; } = notionRawApiService;

    public async Task UpdateTimesheetForTodayVacationAsync()
    {
        var today = DateTime.Today;
        var vacationDay = new DateTime(2025, 12, 10);
        var timeSheetDay = new DateTime(2025, 11, 26);

        var vacation = await NotionRawApiService.GetVacationsByDateAsync(vacationDay);
        if (vacation is null) return;

        var timesheet = await NotionDatabaseService.GetTimesheetByDateAsync(timeSheetDay);
        if (timesheet is null) throw new Exception($"Timesheet not found for date {today:yyyy-MM-dd}. Cannot mark holiday.");
    }

}