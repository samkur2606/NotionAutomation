using Newtonsoft.Json.Linq;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionRawParser
{
    public List<Vacation> ParseVacations(string json)
    {
        var vacations = new List<Vacation>();
        var jObject = JObject.Parse(json);
        var results = jObject["results"]?.Children() ?? Enumerable.Empty<JToken>();

        foreach (var page in results)
        {
            var properties = page["properties"];
            if (properties == null) continue;

            var name = properties[NotionNames.Vacations.Properties.Name]?["title"]?.FirstOrDefault()?["plain_text"]?.ToString();

            var durationProperty = properties[NotionNames.Vacations.Properties.Duration]?["date"];
            
            if (durationProperty is null) throw new Exception("text completing...");

            var startDate = durationProperty["start"]?.ToObject<DateTime?>();
            var endDate = durationProperty["end"]?.ToObject<DateTime?>();

            if (startDate is null) throw new Exception("text completing...");
            if (endDate is null) throw new Exception("text completing...");

            var duration = new DateRange { Start = startDate.Value, End = endDate.Value };
            var statusAsString = properties[NotionNames.Vacations.Properties.Status]?["select"]?["name"]?.ToString();

            if (string.IsNullOrEmpty(statusAsString) || !Enum.TryParse<VacationStatus>(statusAsString, true, out var parsedStatus))
                throw new Exception("text completing...");

            vacations.Add(new Vacation
            {
                Name = name,
                Duration = duration,
                Status = parsedStatus
            });
        }

        return vacations;
    }
}