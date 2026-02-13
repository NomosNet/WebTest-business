using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.BusinessLogic.Mapping;
using WebTest.BusinessLogic.Services;
using WebTest.DataAccess.Repositories;
using WebTest.ViewModels.DTOs.Requests;
using WebTest.ViewModels.DTOs.Responses;

namespace WebTest.Controllers;


[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerService _customerService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderService _orderService;

    public ApiController(
        IProductRepository productRepository,
        ICustomerService customerService,
        ICustomerRepository customerRepository,
        IOrderService orderService)
    {
        _productRepository = productRepository;
        _customerService = customerService;
        _customerRepository = customerRepository;
        _orderService = orderService;
    }

    // ---------- ПРОДУКТЫ ----------

    [HttpGet("products")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var response = products
            .OrderBy(p => p.Name)
            .Select(p => p.ToResponseDto())
            .ToList();

        return Ok(response);
    }

    [HttpGet("products/{id:guid}")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();

        return Ok(product.ToResponseDto());
    }

    // ---------- ЗАКАЗЫ (ДЛЯ ЗАКАЗЧИКА) ----------

    [Authorize(Policy = "CustomerOnly")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid || dto.Items == null || !dto.Items.Any())
        {
            return BadRequest(new { success = false, message = "Пустой заказ" });
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userName = User.Identity?.Name ?? $"customer-{userId}";

            // Получаем или создаём заказчика
            var customer = await _customerService.GetOrCreateCustomerForUserAsync(userId, userName);

            // Создаём заказ через сервис
            var order = await _orderService.CreateOrderAsync(customer.Id, dto.Items);

            return Ok(new
            {
                success = true,
                orderId = order.Id,
                orderNumber = order.OrderNumber,
                status = order.Status
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Получить заказы текущего заказчика.
    /// GET /api/orders?status=...
    /// </summary>
    [HttpGet("orders")]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IActionResult> GetMyOrders([FromQuery] string? status)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customer = await _customerRepository.GetByAppUserIdAsync(userId);

        if (customer == null)
        {
            return Ok(new List<OrderResponseDto>());
        }

        var orders = await _orderService.GetOrdersByCustomerIdAsync(customer.Id, status);
        var response = orders.Select(o => o.ToResponseDto()).ToList();

        return Ok(response);
    }
}

