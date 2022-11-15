using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("v1/categories")]
    public async Task<IActionResult> GetAsync(BlogDataContext context)
    {
        try
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();

            return Ok(categories);
        }
        catch (Exception e)
        {
            return StatusCode(500, "05X04 - Falha interna no servidor");
        }
    }

    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id, BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound();

            return Ok(category);
        }
        catch (Exception e)
        {
            return StatusCode(500, "05X05 - Falha interna no servidor");
        }
    }

    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync(BlogDataContext context, Category category)
    {
        try
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", category);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, "05XE9 - Não foi possível incluir a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "05X10 - Falha interna no servidor");
        }
    }

    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync(int id, BlogDataContext context, Category category)
    {
        try
        {
            var categoryById = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (categoryById is null)
                return NotFound();

            categoryById.Name = category.Name;
            categoryById.Slug = category.Slug;

            context.Categories.Update(categoryById);
            await context.SaveChangesAsync();

            return Ok(category);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, "05XE8 - Não foi possível alterar a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "05X11 - Falha interna no servidor");
        }
    }

    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, "05XE7 - Não foi possível remover a categoria");
        }
        catch (Exception e)
        {
            return StatusCode(500, "05X12 - Falha interna no servidor");
        }
    }
}