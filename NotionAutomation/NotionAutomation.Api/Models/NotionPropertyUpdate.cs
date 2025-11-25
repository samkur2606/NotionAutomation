namespace NotionAutomation.Api.Models;

public class NotionPropertyUpdate
{
    public string Name { get; set; } = null!;
    public object Value { get; set; } = null!;
}