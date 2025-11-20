using Microsoft.AspNetCore.Mvc;

namespace NotionAutomation.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    [HttpGet(Name = "GetData")]
    public string Get()
    {
        return "Hello Notion";
    }
}