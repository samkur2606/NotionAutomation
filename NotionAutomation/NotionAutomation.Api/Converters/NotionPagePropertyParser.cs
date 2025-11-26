using Notion.Client;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionPagePropertyParser
{
    private const string NameProperty = "Name";

    public Guid GetPageId(IWikiDatabase wikiDatabase)
    {
        var page = GetPageOrThrowException(wikiDatabase);
        var id = Guid.Parse(page.Id);
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

    public DateRange GetDateRange(IWikiDatabase wikiDatabase, string propertyName)
    {
        var page = GetPageOrThrowException(wikiDatabase);

        if (page.Properties[propertyName] is not DatePropertyValue datePropertyValue || datePropertyValue.Date is null)
            throw new InvalidOperationException($"Property '{propertyName}' does not exist or has no date value.");

        if (datePropertyValue.Date.Start is null)
            throw new InvalidOperationException($"Property '{propertyName}' must have a Start date.");

        if (datePropertyValue.Date.End is null)
            throw new InvalidOperationException($"Property '{propertyName}' must have a End date.");

        var start = datePropertyValue.Date.Start.Value.DateTime;
        var end = datePropertyValue.Date.End.Value.DateTime;

        var dateRange = new DateRange(start, end);
        return dateRange;
    }
}