namespace NotionAutomation.Api.Models;

public class NotionPropertyUpdate
{
    public NotionPropertyUpdateType Type { get; set; }
    public string Name { get; set; } = null!;
    public object Value { get; set; } = null!;
}