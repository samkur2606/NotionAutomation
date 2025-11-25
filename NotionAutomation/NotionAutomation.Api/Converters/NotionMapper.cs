using Notion.Client;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionMapper(NotionPagePropertyParser notionPagePropertyParser)
{
    public NotionPagePropertyParser NotionPagePropertyParser { get; } = notionPagePropertyParser;

    //public NotionPage MapToNotionPage(IWikiDatabase wikiDatabase)
    //{
    //    var title = NotionPagePropertyParser.GetName(wikiDatabase);
    //    var date = NotionPagePropertyParser.GetDate(wikiDatabase, "MyDate");
    //    var created = NotionPagePropertyParser.GetCreatedTime(wikiDatabase);
    //    var select = NotionPagePropertyParser.GetSelect(wikiDatabase, "MySelect");
    //    var description = NotionPagePropertyParser.GetRichText(wikiDatabase, "MyDescription");
    //    var id = NotionPagePropertyParser.GetPageId(wikiDatabase);

    //    var notionPage = new NotionPage
    //    {
    //        Id = id,
    //        Name = title,
    //        Date = date,
    //        Select = select,
    //        Created = created,
    //        Description = description
    //    };

    //    return notionPage;
    //}

    public Holiday MapToHoliday(IWikiDatabase wikiDatabase)
    {
        var holiday = new Holiday
        {
            Name = NotionPagePropertyParser.GetName(wikiDatabase),
            Date = NotionPagePropertyParser.GetDate(wikiDatabase, NotionSchema.Holidays.Properties.DateName)
        };

        return holiday;
    }

    public TimeSheet MapToTimeSheet(IWikiDatabase wikiDatabase)
    {
        var typeAsString = NotionPagePropertyParser.GetSelect(wikiDatabase, NotionSchema.TimeSheets.Properties.Type);
        var type = ParseEnum<TimeSheetType>(typeAsString);

        var timeSheet = new TimeSheet
        {
            PageId = NotionPagePropertyParser.GetPageId(wikiDatabase),
            Date = NotionPagePropertyParser.GetDate(wikiDatabase, NotionSchema.Holidays.Properties.DateName),
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