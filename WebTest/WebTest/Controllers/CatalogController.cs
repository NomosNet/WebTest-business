using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.BusinessLogic.Mapping;
using WebTest.BusinessLogic.Services;
using WebTest.DataAccess.Repositories;
using WebTest.ViewModels.Cart;
using WebTest.ViewModels.DTOs.Requests;
using WebTest.ViewModels.Order;
using WebTest.ViewModels.Product;

namespace WebTest.Controllers;

[Authorize(Policy = "CustomerOnly")]
public class CatalogController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICartService _cartService;
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly ICustomerRepository _customerRepository;

    public CatalogController(
        IProductRepository productRepository,
        ICartService cartService,
        ICustomerService customerService,
        IOrderService orderService,
        ICustomerRepository customerRepository)
    {
        _productRepository = productRepository;
        _cartService = cartService;
        _customerService = customerService;
        _orderService = orderService;
        _customerRepository = customerRepository;
    }

    // Каталог товаров
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        var viewModel = new ProductListViewModel
        {
            Products = products.Select(p => p.ToViewModel()).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AddToCart([FromForm] Guid productId)
    {
        _cartService.AddToCart(HttpContext.Session, productId);
        return RedirectToAction("Cart");
    }

    public async Task<IActionResult> Cart()
    {
        var cartItems = await _cartService.GetCartItemsAsync(HttpContext.Session);
        var viewModel = new CartViewModel
        {
            Items = cartItems.Select(item => item.product.ToCartItemViewModel(item.quantity)).ToList(),
            Total = cartItems.Sum(item => item.product.Price * item.quantity)
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var cart = _cartService.GetCart(HttpContext.Session);
        if (!cart.Any())
            return RedirectToAction("Cart");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userName = User.Identity?.Name ?? $"customer-{userId}";

        // Получаем или создаём заказчика
        var customer = await _customerService.GetOrCreateCustomerForUserAsync(userId, userName);

        // Преобразуем корзину в DTO
        var orderItems = cart.Select(kvp => new CreateOrderItemDto
        {
            ProductId = kvp.Key,
            Quantity = kvp.Value
        }).ToList();

        // Создаём заказ через сервис
        await _orderService.CreateOrderAsync(customer.Id, orderItems);

        // Очищаем корзину
        _cartService.ClearCart(HttpContext.Session);

        return RedirectToAction("MyOrders");
    }

    public async Task<IActionResult> MyOrders(string? status)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customer = await _customerRepository.GetByAppUserIdAsync(userId);

        if (customer == null)
        {
            return View(new OrderListViewModel { StatusFilter = status });
        }

        var orders = await _orderService.GetOrdersByCustomerIdAsync(customer.Id, status);
        var viewModel = new OrderListViewModel
        {
            Orders = orders.Select(o => o.ToViewModel()).ToList(),
            StatusFilter = status
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOrder(Guid id, string? status)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customer = await _customerRepository.GetByAppUserIdAsync(userId);
        if (customer == null)
            return RedirectToAction("MyOrders", new { status });

        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null || order.CustomerId != customer.Id)
            return RedirectToAction("MyOrders", new { status });

        try
        {
            await _orderService.DeleteOrderAsync(id);
        }
        catch (InvalidOperationException)
        {
            // Заказ нельзя удалить (статус "Выполняется")
        }

        return RedirectToAction("MyOrders", new { status });
    }
}

