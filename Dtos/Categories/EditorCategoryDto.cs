using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Categories;

public class EditorCategoryDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 40 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O slug é obrigatório")]
    public string Slug { get; set; }
}