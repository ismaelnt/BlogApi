using BlogApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiKey]
[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok();
    }
}