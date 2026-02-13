namespace WebTest.ViewModels.Order;

public class OrderListViewModel
{
    public List<OrderViewModel> Orders { get; set; } = new();
    public string? StatusFilter { get; set; }
}
