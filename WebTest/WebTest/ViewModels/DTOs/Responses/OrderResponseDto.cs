namespace WebTest.ViewModels.DTOs.Responses;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ShipmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponseDto> Items { get; set; } = new();
}
