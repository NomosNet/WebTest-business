using System.ComponentModel.DataAnnotations;

namespace WebTest.ViewModels.DTOs.Requests;

public class AddToCartDto
{
    [Required]
    public Guid ProductId { get; set; }
}
