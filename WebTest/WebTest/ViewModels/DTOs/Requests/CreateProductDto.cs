using System.ComponentModel.DataAnnotations;

namespace WebTest.ViewModels.DTOs.Requests;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
}
