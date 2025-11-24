using Notion.Client;

namespace NotionAutomation.Api.Converters;

public class NotionPageUpdateBuilder
{
    public PagesUpdateParameters CreatePagesUpdateParameters(Dictionary<string, object> properties)
    {

        var notionProperties = new Dictionary<string, PropertyValue>();
        foreach (var (propertyName, propertyValue) in properties)
        {
            notionProperties[propertyName] = propertyValue switch
            {
                SelectOption selectOption => new SelectPropertyValue { Select = selectOption },
                DateTimeOffset dateTimeOffset => new DatePropertyValue { Date = new Date { Start = dateTimeOffset } },
                _ => throw new ArgumentException($"Unsupported type for '{propertyName}'.")
            };
        }

        var pagesUpdateParameters = new PagesUpdateParameters { Properties = notionProperties };
        return pagesUpdateParameters;
    }
}