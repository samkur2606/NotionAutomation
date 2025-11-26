using Notion.Client;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionMapper(NotionParser notionParser)
{
    public NotionParser NotionParser { get; } = notionParser;

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
}