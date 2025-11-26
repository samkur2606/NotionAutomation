using System.Net.Http.Headers;
using System.Text;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionRawApiService
{
    public NotionRawApiService(HttpClient httpClient, AppSettings settings, ConfigurationHelper configurationHelper)
    {
        HttpClient = httpClient;
        AppSettings = settings;
        ConfigurationHelper = configurationHelper;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);
    }

    private HttpClient HttpClient { get; }
    private AppSettings AppSettings { get; }
    private ConfigurationHelper ConfigurationHelper { get; }

    public async Task<string> GetDatabaseAsync(string notionDatabaseId)
    {
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabaseId}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetVacationsByDateAsync(DateTime dateTime)
    {
        var databaseId = ConfigurationHelper.GetDatabaseId(NotionNames.Vacations.Database);
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{databaseId}/query";
        var isoDate = dateTime.ToString("yyyy-MM-dd");

        var jsonBody = $@"
        {{
            ""filter"": {{
                ""and"": [
                    {{
                        ""property"": ""Duration"",
                        ""date"": {{
                            ""on_or_before"": ""{isoDate}""
                        }}
                    }},
                    {{
                        ""property"": ""Duration"",
                        ""date"": {{
                            ""on_or_after"": ""{isoDate}""
                        }}
                    }}
                ]
            }}
        }}";

        var response = await HttpClient.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var content =  await response.Content.ReadAsStringAsync();
        return content;
    }

}