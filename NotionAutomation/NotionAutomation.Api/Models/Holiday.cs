namespace NotionAutomation.Api.Models;

public class Holiday
{
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public bool IsBusinessDay { get; set; }
}