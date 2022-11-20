using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Accounts;

public class RegisterDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório"), EmailAddress(ErrorMessage = "O E-mail é inválido")]
    public string Email { get; set; }
}