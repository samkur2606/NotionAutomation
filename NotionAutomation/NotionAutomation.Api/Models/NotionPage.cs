namespace NotionAutomation.Api.Models;

public class NotionPage
{
    public string? Name { get; set; }
    public DateTimeOffset? Date { get; set; }
    public string? Select { get; set; }
    public DateTimeOffset? Created { get; set; }
    public string? Description { get; set; }
}