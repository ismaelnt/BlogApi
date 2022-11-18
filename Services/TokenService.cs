using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BlogApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services;

public class TokenService
{
    public string GenerateToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}