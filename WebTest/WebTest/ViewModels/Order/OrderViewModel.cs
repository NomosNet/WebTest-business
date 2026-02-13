namespace WebTest.ViewModels.Order;

public class OrderViewModel
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ShipmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItemViewModel> Items { get; set; } = new();
    public decimal Total { get; set; }
}
