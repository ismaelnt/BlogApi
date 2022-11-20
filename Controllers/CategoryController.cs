using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Dtos.Categories;
using BlogApi.Extensions;
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

            return Ok(new ResultDto<List<Category>>(categories));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultDto<List<Category>>("05X04 - Falha interna no servidor"));
        }
    }

    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id, BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound(new ResultDto<Category>("Conteúdo não encontrado"));

            return Ok(new ResultDto<Category>(category));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultDto<Category>("05X05 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync(BlogDataContext context, EditorCategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultDto<Category>(ModelState.GetErrors()));

        try
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Slug = categoryDto.Slug.ToLower()
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", new ResultDto<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultDto<Category>("05XE9 - Não foi possível incluir a categoria"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultDto<Category>("05X10 - Falha interna no servidor"));
        }
    }

    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync(int id, BlogDataContext context, EditorCategoryDto categoryDto)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound(new ResultDto<Category>("Conteúdo não encontrado"));

            category.Name = categoryDto.Name;
            category.Slug = categoryDto.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(new ResultDto<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultDto<Category>("05XE8 - Não foi possível alterar a categoria"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultDto<Category>("05X11 - Falha interna no servidor"));
        }
    }

    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound(new ResultDto<Category>("Conteúdo não encontrado"));

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(new ResultDto<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultDto<Category>("05XE7 - Não foi possível remover a categoria"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultDto<Category>("05X12 - Falha interna no servidor"));
        }
    }
}