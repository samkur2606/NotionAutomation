using Notion.Client;

namespace NotionAutomation.Api.Services;

public class NotionDatabaseService(INotionClient notionClient)
{
    public INotionClient NotionClient { get; } = notionClient;

    public async Task<List<IWikiDatabase>> QueryDatabaseAsync(string notionDatabaseId, int pageSize)
    {
        var result = await NotionClient.Databases.QueryAsync(notionDatabaseId, new DatabasesQueryParameters { PageSize = pageSize });
        return result.Results;
    }
}