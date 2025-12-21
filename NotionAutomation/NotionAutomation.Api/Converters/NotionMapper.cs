using Newtonsoft.Json.Linq;
using Notion.Client;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionMapper(NotionParser notionParser)
{
    public NotionParser NotionParser { get; } = notionParser;

    public Vacation MapToVacation(IWikiDatabase? wikiDatabase)
    {
        if (wikiDatabase is null) throw new ArgumentNullException(nameof(wikiDatabase));

        var status = NotionParser.GetSelect(wikiDatabase, "Status");
        var vacation = new Vacation
        {
            Name = NotionParser.GetName(wikiDatabase),
            Status = ParseEnum<VacationStatus>(status),
            Duration = NotionParser.GetDateRange(wikiDatabase, "Duration")

        };
        return vacation;
    }

    public Holiday MapToHoliday(IWikiDatabase wikiDatabase)
    {
        var holiday = new Holiday
        {
            Name = NotionParser.GetName(wikiDatabase),
            Date = NotionParser.GetDate(wikiDatabase, NotionNames.Holidays.Properties.Date)
        };

        return holiday;
    }

    public TimeSheet MapToTimeSheet(IWikiDatabase wikiDatabase)
    {
        var typeAsString = NotionParser.GetSelect(wikiDatabase, NotionNames.TimeSheets.Properties.Type);
        var type = ParseEnum<TimeSheetType>(typeAsString);

        var timeSheet = new TimeSheet
        {
            PageId = NotionParser.GetPageId(wikiDatabase),
            Date = NotionParser.GetDate(wikiDatabase, NotionNames.Holidays.Properties.Date),
            Type = type
        };

        return timeSheet;
    }

    private static T? ParseEnum<T>(string value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var cleaned = value.Replace(" ", "");

        if (Enum.TryParse<T>(cleaned, ignoreCase: true, out var result))
            return result;

        return null;
    }

    public Page ToSafePage(JToken pageToken, params string[] unsupportedPropertyTypes)
    {
        if (pageToken["properties"] is not JObject properties)
            throw new InvalidOperationException("Page has no properties object.");

        var typesToRemove = new HashSet<string>(unsupportedPropertyTypes, StringComparer.OrdinalIgnoreCase);

        var toRemove = properties.Properties()
            .Where(p => p.Value?["type"] != null && typesToRemove.Contains(p.Value["type"]!.ToString()))
            .Select(p => p.Name)
            .ToList();

        foreach (var name in toRemove)
        {
            properties.Remove(name);
        }

        return pageToken.ToObject<Page>()!;
    }
}