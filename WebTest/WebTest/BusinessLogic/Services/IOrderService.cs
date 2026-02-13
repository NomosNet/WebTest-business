using WebTest.Models;
using WebTest.ViewModels.DTOs.Requests;

namespace WebTest.BusinessLogic.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Guid customerId, List<CreateOrderItemDto> items);
    Task<bool> CanDeleteOrderAsync(Guid orderId);
    Task DeleteOrderAsync(Guid orderId);
    Task SetOrderStatusAsync(Guid orderId, string status, DateTime? shipmentDate = null);
    Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId, string? status = null);
    Task<Order?> GetOrderByIdAsync(Guid id);
}
