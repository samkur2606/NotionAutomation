namespace NotionAutomation.Api.Models;

public class TimeSheet
{
    public string? PageId { get; set; }
    public DateTimeOffset? Date { get; set; }
    public TimeSheetType? Type { get; set; }
}