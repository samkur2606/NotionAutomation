using Notion.Client;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Converters;

public class NotionPageUpdateBuilder
{
    public PagesUpdateParameters CreatePagesUpdateParameters(params NotionPropertyUpdate[] properties)
    {
        var notionProperties = CreateNotionProperties(properties);
        var pagesUpdateParameters = new PagesUpdateParameters { Properties = notionProperties };
        return pagesUpdateParameters;
    }

    public PagesCreateParameters CreatePagesCreateParameters(Guid parentDatabaseId, params NotionPropertyUpdate[] properties)
    {
        var notionProperties = CreateNotionProperties(properties);

        var pagesCreateParameters = new PagesCreateParameters
        {
            Parent = new DatabaseParentInput
            {
                DatabaseId = parentDatabaseId.ToString()
            },
            Properties = notionProperties
        };

        return pagesCreateParameters;
    }


    private Dictionary<string, PropertyValue> CreateNotionProperties(NotionPropertyUpdate[] properties)
    {
        var notionProperties = new Dictionary<string, PropertyValue>();

        foreach (var property in properties)
        {
            notionProperties[property.Name] = property.Type switch
            {
                NotionPropertyUpdateType.Title => CreateTitlePropertyValue((string)property.Value),
                NotionPropertyUpdateType.Select => CreateSelectPropertyValue((Enum)property.Value),
                NotionPropertyUpdateType.Date => CreateDatePropertyValue((DateTimeOffset)property.Value),
                _ => throw new ArgumentException($"Unsupported type for '{property.Name}'. Only Title, Select, and Date are supported.")
            };
        }

        return notionProperties;
    }

    public NotionPropertyUpdate CreateNotionPropertyUpdate(NotionPropertyUpdateType type, string propertyName, object propertyValue)
    {
        var property = new NotionPropertyUpdate
        {
            Type = type,
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

    private TitlePropertyValue CreateTitlePropertyValue(string content)
    {
        var titlePropertyValue = new TitlePropertyValue
        {
            Title = [new RichTextText { Text = new Text { Content = content } }]
        };
        return titlePropertyValue;
    }
}