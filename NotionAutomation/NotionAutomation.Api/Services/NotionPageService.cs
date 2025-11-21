using Notion.Client;
using NotionAutomation.Api.Converters;

namespace NotionAutomation.Api.Services;

public class NotionPageService(INotionClient notionClient)
{
    public INotionClient NotionClient { get; } = notionClient;


    public async Task<Page> UpdatePageAsync(string pageId, PagesUpdateParameters parameters)
    {
        var updatePage = await NotionClient.Pages.UpdateAsync(pageId, parameters);
        return updatePage;
    }
}