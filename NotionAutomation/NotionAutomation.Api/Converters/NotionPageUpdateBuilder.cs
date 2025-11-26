using Notion.Client;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionPageUpdateBuilder
{
    public PagesUpdateParameters CreatePagesUpdateParameters(params NotionPropertyUpdate[] properties)
    {
        var notionProperties = new Dictionary<string, PropertyValue>();

        foreach (var property in properties)
        {
            notionProperties[property.Name] = property.Value switch
            {
                Enum enumValue => CreateSelectPropertyValue(enumValue),
                DateTimeOffset date => CreateDatePropertyValue(date),
                _ => throw new ArgumentException($"Unsupported type for '{property.Name}'. Only Enum and DateTimeOffset are supported.")
            };
        }

        var pagesUpdateParameters = new PagesUpdateParameters { Properties = notionProperties };
        return pagesUpdateParameters;
    }
    
    public NotionPropertyUpdate CreateNotionPropertyUpdate(string propertyName, object propertyValue)
    {
        var property = new NotionPropertyUpdate
        {
            Name = propertyName,
            Value = propertyValue
        };

        return property;
    }

    private SelectPropertyValue CreateSelectPropertyValue(Enum value)
    {
        var selectOption = new SelectOption { Name = value.GetDescription() };
        var selectProperty = new SelectPropertyValue { Select = selectOption };
        return selectProperty;
    }

    private DatePropertyValue CreateDatePropertyValue(DateTimeOffset value)
    {
        var date = new Date { Start = value };
        var dateProperty = new DatePropertyValue { Date = date };
        return dateProperty;
    }
}