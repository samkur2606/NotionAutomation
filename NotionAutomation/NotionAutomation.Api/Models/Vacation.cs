namespace NotionAutomation.Api.Models;

public class Vacation
{
    public string? Name { get; set; }
    public DateRange? Duration { get; set; }
    public VacationStatus? Status { get; set; }
}