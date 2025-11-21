using NotionAutomation.Api.Models;
using System.Net.Http.Headers;
using System.Text;

namespace NotionAutomation.Api.Services;

public class NotionRawApiService
{
    public HttpClient HttpClient { get; }
    public AppSettings AppSettings { get; }

    public NotionRawApiService(HttpClient httpClient, AppSettings settings)
    {
        HttpClient = httpClient;
        AppSettings = settings;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);
    }

    public async Task<string> QueryDatabaseRawAsync(string notionDatabaseId)
    {
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabaseId}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}