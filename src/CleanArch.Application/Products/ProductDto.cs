using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Products;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo Name es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
