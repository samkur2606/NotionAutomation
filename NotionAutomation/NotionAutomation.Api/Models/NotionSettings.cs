namespace NotionAutomation.Api.Models;

public class NotionSettings
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = string.Empty;
    public string IntegrationToken { get; set; } = string.Empty;
    public List<NotionDatabase> Databases { get; set; } = new();
}