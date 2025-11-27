namespace NotionAutomation.Api.Models;

public class NotionLog
{
    public DateTimeOffset? LoggingTime { get; set; }
    public DateTimeOffset? Created { get; set; }
    public NotionLogLevel? LogLevel { get; set; }
    public string? Source { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? AdditionalData { get; set; }
}
