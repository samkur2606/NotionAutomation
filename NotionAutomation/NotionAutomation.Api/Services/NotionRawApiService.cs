using System.Net.Http.Headers;
using System.Text;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionRawApiService
{
    public NotionRawApiService(HttpClient httpClient, AppSettings settings, ConfigurationHelper configurationHelper, NotionRawParser notionRawParser)
    {
        HttpClient = httpClient;
        AppSettings = settings;
        ConfigurationHelper = configurationHelper;
        NotionRawParser = notionRawParser;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.Notion.IntegrationToken);
        HttpClient.DefaultRequestHeaders.Add("Notion-Version", AppSettings.Notion.ApiVersion);
    }

    private HttpClient HttpClient { get; }
    private AppSettings AppSettings { get; }
    private ConfigurationHelper ConfigurationHelper { get; }
    private NotionRawParser NotionRawParser { get; }

    public async Task<string> GetDatabaseAsync(string notionDatabaseId)
    {
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{notionDatabaseId}/query";
        var response = await HttpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<Vacation>> GetVacationsByDateAsync(DateTime dateTime)
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
        var content = await response.Content.ReadAsStringAsync();
        var vacations = NotionRawParser.ParseVacations(content);
        return vacations;
    }

    public async Task<List<Vacation>> GetVacationsByMonthAsync(int year, int month)
    {
        var databaseId = ConfigurationHelper.GetDatabaseId(NotionNames.Vacations.Database);
        var url = $"{AppSettings.Notion.ApiBaseUrl}/{databaseId}/query";

        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var isoDateStart = monthStart.ToString("yyyy-MM-dd");
        var isoDateEnd = monthEnd.ToString("yyyy-MM-dd");

        var jsonBody = $@"
        {{
            ""filter"": {{
                ""and"": [
                    {{
                        ""property"": ""{NotionNames.Vacations.Properties.DateStart}"",
                        ""date"": {{
                            ""on_or_before"": ""{isoDateEnd}""
                        }}
                    }},
                    {{
                        ""property"": ""{NotionNames.Vacations.Properties.DateEnd}"",
                        ""date"": {{
                            ""on_or_after"": ""{isoDateStart}""
                        }}
                    }}
                ]
            }}
        }}";

        var response = await HttpClient.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var vacations = NotionRawParser.ParseVacations(content);

        return vacations;
    }

    public async Task<int> GetVacationDaysInMonthAsync(int year, int month)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var vacations = await GetVacationsByMonthAsync(year, month);

        var totalDays = 0;

        foreach (var vacation in vacations)
        {
            if (vacation.Duration is null) throw new Exception($"Vacation '{vacation.Name ?? "Unnamed"}' has no Duration set. Cannot calculate vacation days.");

            var vacationStart = vacation.Duration.Value.Start;
            var vacationEnd = vacation.Duration.Value.End;

            var overlapStart = vacationStart > monthStart ? vacationStart : monthStart;
            var overlapEnd = vacationEnd < monthEnd ? vacationEnd : monthEnd;

            if (overlapStart <= overlapEnd)
            {
                totalDays += (int)(overlapEnd - overlapStart).TotalDays + 1;
            }
        }

        return totalDays;
    }
}