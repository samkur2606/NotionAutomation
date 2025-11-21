namespace NotionAutomation.Api.Models;

public class AppSettings
{
    public NotionSettings Notion { get; set; } = new();
}