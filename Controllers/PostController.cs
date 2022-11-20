using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Dtos.Posts;
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

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .ThenInclude(u => u.Roles)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
                return NotFound(new ResultDto<Post>("Conteúdo não encontrado"));

            return Ok(new ResultDto<Post>(post));
        }
        catch
        {
            return StatusCode(500, new ResultDto<Post>("05X04 - Falha interna no servidor"));
        }
    }

    [HttpGet("v1/posts/category/{category}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromRoute] string category,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 25)
    {
        try
        {
            var count = await _context.Posts.AsNoTracking().CountAsync();
            var posts = await _context
                .Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Category.Slug == category)
                .Select(x => new GetPostsDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
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
            return StatusCode(500, new ResultDto<List<Category>>("05X04 - Falha interna no servidor"));
        }
    }
}