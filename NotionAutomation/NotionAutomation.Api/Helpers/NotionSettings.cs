namespace NotionAutomation.Api.Helpers;

public class NotionSettings
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string IntegrationToken { get; set; } = string.Empty;
    public Dictionary<string, string> Databases { get; set; } = new();
}