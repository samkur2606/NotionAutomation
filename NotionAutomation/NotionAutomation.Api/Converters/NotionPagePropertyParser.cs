using Notion.Client;

namespace NotionAutomation.Api.Converters;

public class NotionPagePropertyParser
{
    private const string NameProperty = "Name";

    public string GetPageId(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        var id = page.Id; 
        return id;
    }

    public string GetRichText(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);

        if (page.Properties[propertyName] is RichTextPropertyValue { RichText: not null } richTextPropertyValue)
            return string.Join(" ", richTextPropertyValue.RichText.Select(rt => rt.PlainText));

        return string.Empty;
    }

    public string GetSelect(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);

        if (page.Properties[propertyName] is SelectPropertyValue { Select: not null } selectPropertyValue) return selectPropertyValue.Select.Name;

        return string.Empty;
    }

    public DateTimeOffset GetCreatedTime(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        return page.CreatedTime;
    }

    public string GetName(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        if (page.Properties[NameProperty] is TitlePropertyValue titlePropertyValue)
            return titlePropertyValue.Title.FirstOrDefault()?.PlainText ?? string.Empty;

        return string.Empty;
    }

    public DateTimeOffset? GetDate(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        if (page.Properties[propertyName] is DatePropertyValue { Date: not null } datePropertyValue)
            return datePropertyValue.Date.Start;

        return null;
    }

    private Page GetPageOrThrowException(IWikiDatabase wikiDatabase)
    {
        if (wikiDatabase is not Page page)
            throw new ArgumentException("The provided object is not a Page.", nameof(wikiDatabase));
        return page;
    }
}