using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTest.BusinessLogic.Mapping;
using WebTest.BusinessLogic.Services;
using WebTest.DataAccess.Repositories;
using WebTest.Models;
using WebTest.ViewModels.DTOs.Requests;
using WebTest.ViewModels.Manager;
using WebTest.ViewModels.Order;

namespace WebTest.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class ManagerController : Controller
{
    private readonly IProductService _productService;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;

    public ManagerController(
        IProductService productService,
        IProductRepository productRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IOrderService orderService)
    {
        _productService = productService;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        var users = await _userRepository.GetAllAsync();
        var orders = await _orderRepository.GetAllWithIncludesAsync();

        var viewModel = new ManagerIndexViewModel
        {
            Products = products.OrderBy(p => p.Name).Select(p => p.ToViewModel()).ToList(),
            Users = users.Select(u => u.ToViewModel()).ToList(),
            Orders = orders.Select(o => o.ToViewModel()).ToList()
        };

        return View(viewModel);
    }

    // ----- ТОВАРЫ -----
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] string name, [FromForm] string? description, [FromForm] decimal price)
    {
        var dto = new CreateProductDto
        {
            Name = name,
            Description = description,
            Price = price
        };

        await _productService.CreateProductAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct([FromForm] Guid id, [FromForm] string name, [FromForm] string? description, [FromForm] decimal price)
    {
        var dto = new EditProductDto
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price
        };

        await _productService.UpdateProductAsync(dto);
        return RedirectToAction("Index");
    }

    // AJAX: получить товар в JSON
    [HttpGet]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        return Json(new
        {
            id = product.Id,
            name = product.Name,
            description = product.Category,
            price = product.Price
        });
    }

    // AJAX: сохранить товар из JSON
    [HttpPost]
    public async Task<IActionResult> SaveProduct([FromBody] EditProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Некорректные данные" });
        }

        try
        {
            var product = await _productService.UpdateProductAsync(dto);
            return Ok(new
            {
                success = true,
                message = "Товар успешно сохранён",
                product = new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Category,
                    price = product.Price
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction("Index");
    }

    // ----- ПОЛЬЗОВАТЕЛИ -----
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromForm] string userName, [FromForm] string password, [FromForm] UserRole role)
    {
        var user = new AppUser
        {
            UserName = userName,
            Password = password,
            Role = role
        };
        await _userRepository.AddAsync(user);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EditUser([FromForm] int id, [FromForm] string userName, [FromForm] string password, [FromForm] UserRole role)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            user.UserName = userName;
            user.Password = password;
            user.Role = role;
            await _userRepository.UpdateAsync(user);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userRepository.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    // ----- ЗАКАЗЫ -----
    [HttpPost]
    public async Task<IActionResult> SetInProgress([FromForm] Guid id, [FromForm] DateTime deliveryDate)
    {
        try
        {
            await _orderService.SetOrderStatusAsync(id, OrderStatuses.InProgress, deliveryDate);
        }
        catch (InvalidOperationException)
        {
            // Заказ не найден
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SetCompleted([FromForm] Guid id)
    {
        try
        {
            await _orderService.SetOrderStatusAsync(id, OrderStatuses.Completed);
        }
        catch (InvalidOperationException)
        {
            // Заказ не найден
        }
        return RedirectToAction("Index");
    }
}

