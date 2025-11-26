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
            
            if (durationProperty is null) throw new Exception($"Vacation {NotionNames.Vacations.Properties.Duration} property is missing.");

            var startDate = durationProperty["start"]?.ToObject<DateTime?>();
            var endDate = durationProperty["end"]?.ToObject<DateTime?>();

            if (startDate is null) throw new Exception("Vacation 'Start Date' is missing or invalid."); ;
            if (endDate is null) throw new Exception("Vacation 'End Date' is missing or invalid."); ;

            var duration = new DateRange { Start = startDate.Value, End = endDate.Value };
            var statusAsString = properties[NotionNames.Vacations.Properties.Status]?["select"]?["name"]?.ToString();

            if (string.IsNullOrEmpty(statusAsString))
                throw new Exception($"Vacation '{NotionNames.Vacations.Properties.Status}' property is missing or empty.");

            if (!Enum.TryParse<VacationStatus>(statusAsString, true, out var parsedStatus))
                throw new Exception($"Vacation '{NotionNames.Vacations.Properties.Status}' value '{statusAsString}' is not a valid {nameof(VacationStatus)}.");

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