namespace NotionAutomation.Api.Helpers;

public static class JsonHelper
{
    public static string PrettyJson(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var prettyJson = System.Text.Json.JsonSerializer.Serialize(
            doc.RootElement,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
        );

        return prettyJson;
    }
}