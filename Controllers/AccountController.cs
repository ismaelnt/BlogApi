using System.Text.RegularExpressions;
using BlogApi.Data;
using BlogApi.Dtos;
using BlogApi.Dtos.Accounts;
using BlogApi.Extensions;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogApi.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private readonly BlogDataContext _context;
    private readonly TokenService _tokenService;

    public AccountController(BlogDataContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("v1/accounts")]
    public async Task<IActionResult> PostAsync(RegisterDto modelDto, EmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultDto<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = modelDto.Name,
            Email = modelDto.Email,
            Slug = modelDto.Email.Replace("@", "-").Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            emailService.Send(
                user.Name,
                user.Email,
                "Bem vindo ao blog!",
                $"Sua senha é <strong>{password}</strong>"
            );

            return Ok(new ResultDto<dynamic>(new
            {
                user = user.Email
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultDto<string>("05X99 - Este E-mail já está cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultDto<string>("05X04 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> LoginAsync(LoginDto modelDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultDto<string>(ModelState.GetErrors()));

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Email == modelDto.Email);

        if (user is null)
            return StatusCode(401, new ResultDto<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, modelDto.Password))
            return StatusCode(401, new ResultDto<string>("Usuário ou senha inválidos"));

        try
        {
            var token = _tokenService.GenerateToken(user);
            return Ok(new ResultDto<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultDto<string>("05X04 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/accounts/upload-image"), Authorize]
    public async Task<IActionResult> UploadImageAsync(UploadImageDto modelDto)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(modelDto.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch
        {
            return StatusCode(500, new ResultDto<string>("05X04 - Falha interna no servidor"));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

        if (user is null)
            return NotFound(new ResultDto<User>("Usuário não encontrado"));

        user.Image = $"https://localhost:0000/images/{fileName}";

        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return StatusCode(500, new ResultDto<string>("05X04 - Falha interna no servidor"));
        }

        return Ok(new ResultDto<string>("Imagem alterada com sucesso!", null));
    }
}