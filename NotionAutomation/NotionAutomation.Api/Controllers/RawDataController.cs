using Microsoft.AspNetCore.Mvc;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Services;

namespace NotionAutomation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RawDataController(NotionRawApiService notionRawApiService) : ControllerBase
{
    public NotionRawApiService NotionRawApiService { get; } = notionRawApiService;

    [HttpGet(Name = "GetRawData")]
    public async Task<string> Get(string databaseId)
    {
        var rawData = await NotionRawApiService.QueryDatabaseRawAsync(databaseId);
        var prettyJsonRawData = JsonHelper.PrettyJson(rawData);
        return prettyJsonRawData;
    }
}