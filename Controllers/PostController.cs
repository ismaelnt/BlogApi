using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Models;
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
    public async Task<IActionResult> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 25)
    {
        try
        {
            var count = await _context.Posts.AsNoTracking().CountAsync();
            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(p => p.LastUpdateDate)
                .ToListAsync();

            return Ok(new ResultDto<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch
        {
            return StatusCode(500, new ResultDto<List<Post>>("05X04 - Falha interna no servidor"));
        }
    }
}