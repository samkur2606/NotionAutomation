using Notion.Client;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionMapper(NotionPropertyParser notionPropertyParser)
{
    public NotionPropertyParser NotionPropertyParser { get; } = notionPropertyParser;

    //public NotionPage MapToNotionPage(IWikiDatabase wikiDatabase)
    //{
    //    var title = NotionPropertyParser.GetNotionPageName(wikiDatabase);
    //    var date = NotionPropertyParser.GetNotionPageDate(wikiDatabase, "MyDate");
    //    var created = NotionPropertyParser.GetNotionPageCreatedTime(wikiDatabase);
    //    var select = NotionPropertyParser.GetNotionPageSelect(wikiDatabase, "MySelect");
    //    var description = NotionPropertyParser.GetNotionPageRichText(wikiDatabase, "MyDescription");
    //    var id = NotionPropertyParser.GetNotionPageId(wikiDatabase);

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
            Name = NotionPropertyParser.GetNotionPageName(wikiDatabase),
            Date = NotionPropertyParser.GetNotionPageDate(wikiDatabase, NotionSchema.Holidays.Properties.DateName)
        };

        return holiday;
    }
}