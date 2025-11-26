using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class DiscordNotificationService(HttpClient httpClient, AppSettings appSettings) : INotificationService
{
    private HttpClient HttpClient { get; } = httpClient;
    private AppSettings AppSettings { get; } = appSettings;

    public Task NotifySuccess(string message)
    {
        return Send($"✅ **Success**\n{message}");
    }

    public Task NotifyError(string inputMessage, Exception? ex = null)
    {
        var message = $"❌ **Error**\n{inputMessage}";

        if (ex != null)
            message += $"\n\n```\n{ex.Message}\n{ex.StackTrace}\n```";

        return Send(message);
    }

    private async Task Send(string content)
    {
        var webhookUrl = AppSettings.DiscordWebhookUrl;
        var response = await HttpClient.PostAsJsonAsync(webhookUrl, new { content });

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"Discord webhook failed: {response.StatusCode} - {body}");
        }
    }
}