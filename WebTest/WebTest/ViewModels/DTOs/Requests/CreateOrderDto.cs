using System.ComponentModel.DataAnnotations;

namespace WebTest.ViewModels.DTOs.Requests;

public class CreateOrderDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
