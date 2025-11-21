using Notion.Client;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Models;

namespace NotionAutomation.Api.Services;

public class NotionPageService(INotionClient notionClient, NotionPageMapper mapper)
{
    public INotionClient NotionClient { get; } = notionClient;
    public NotionPageMapper Mapper { get; } = mapper;


    public async Task<NotionPage> UpdatePageAsync(string pageId, PagesUpdateParameters parameters)
    {
        var updated = await NotionClient.Pages.UpdateAsync(pageId, parameters);
        var notionPage = Mapper.MapToNotionPage(updated);
        return notionPage;
    }
}