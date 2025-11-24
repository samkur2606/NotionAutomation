using System.Net.Http.Headers;
using System.Text;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionRawApiService
{
    public NotionRawApiService(HttpClient httpClient, AppSettings settings)
    {
        HttpClient = httpClient;
        AppSettings = settings;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);
    }

    public HttpClient HttpClient { get; }
    public AppSettings AppSettings { get; }

    public async Task<string> QueryDatabaseRawAsync(string notionDatabaseId)
    {
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabaseId}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}