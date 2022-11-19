using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AccountController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("v1/login")]
    public IActionResult Login()
    {
        var token = _tokenService.GenerateToken(null);

        return Ok(token);
    }

    [HttpGet("v1/user"), Authorize(Roles = "user")]
    public IActionResult GetUser()
    {
        return Ok(User.Identity.Name);
    }

    [HttpGet("v1/author"), Authorize(Roles = "author")]
    public IActionResult GetAuthor()
    {
        return Ok(User.Identity.Name);
    }

    [HttpGet("v1/admin"), Authorize(Roles = "admin")]
    public IActionResult GetAdmin()
    {
        return Ok(User.Identity.Name);
    }
}