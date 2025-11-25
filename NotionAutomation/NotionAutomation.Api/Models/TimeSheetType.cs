using System.ComponentModel;

namespace NotionAutomation.Api.Models;

public enum TimeSheetType
{
    [Description("Business Day")] BusinessDay,
    [Description("Vacation Day")] VacationDay,
    [Description("Holiday")] Holiday,
    [Description("Sick Day")] SickDay,
    [Description("Weekend")] Weekend
}