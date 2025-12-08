namespace NotionAutomation.Api.Helpers;

public static class DiscordUrlExtractor
{
    public static ulong ExtractWebhookId(string webhookUrl)
    {
        var parts = webhookUrl.Split('/');
        var webhookId = ulong.Parse(parts[^2]);
        return webhookId;
    }

    public static string ExtractWebhookToken(string webhookUrl)
    {
        var parts = webhookUrl.Split('/');
        var webhookId = parts[^1];
        return webhookId;
    }
}