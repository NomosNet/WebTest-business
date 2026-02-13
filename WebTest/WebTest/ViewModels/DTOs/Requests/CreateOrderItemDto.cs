using System.ComponentModel.DataAnnotations;

namespace WebTest.ViewModels.DTOs.Requests;

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
    public int Quantity { get; set; }
}
