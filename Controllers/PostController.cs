using BlogApi.Data;
using BlogApi.Dtos.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    private readonly BlogDataContext _context;

    public PostController(BlogDataContext context)
    {
        _context = context;
    }

    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync()
    {
        var posts = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .ToListAsync();

        return Ok(posts);
    }
}