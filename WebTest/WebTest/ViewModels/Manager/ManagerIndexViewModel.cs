using WebTest.ViewModels.Order;
using WebTest.ViewModels.Product;

namespace WebTest.ViewModels.Manager;

public class ManagerIndexViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();
    public List<UserViewModel> Users { get; set; } = new();
    public List<OrderViewModel> Orders { get; set; } = new();
}
