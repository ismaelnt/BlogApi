using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Accounts;

public class UploadImageDto
{
    [Required(ErrorMessage = "Imagem inválida")]
    public string Base64Image { get; set; }
}