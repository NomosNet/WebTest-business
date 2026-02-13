using WebTest.DataAccess.Repositories;
using WebTest.Models;
using WebTest.ViewModels.DTOs.Requests;

namespace WebTest.BusinessLogic.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Order> CreateOrderAsync(Guid customerId, List<CreateOrderItemDto> items)
    {
        if (items == null || !items.Any())
            throw new InvalidOperationException("Order must contain at least one item");

        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds);

        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Some products not found");

        var maxOrderNumber = await _orderRepository.GetMaxOrderNumberAsync() ?? 0;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            ShipmentDate = null,
            OrderNumber = maxOrderNumber + 1,
            Status = OrderStatuses.New,
            Items = items.Select(i =>
            {
                var product = products.First(p => p.Id == i.ProductId);
                return new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemId = product.Id,
                    ItemsCount = i.Quantity,
                    ItemPrice = product.Price
                };
            }).ToList()
        };

        return await _orderRepository.AddAsync(order);
    }

    public async Task<bool> CanDeleteOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return false;

        // Нельзя удалять заказы со статусом "Выполняется"
        return order.Status != OrderStatuses.InProgress;
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        if (!await CanDeleteOrderAsync(orderId))
            throw new InvalidOperationException("Cannot delete order with status 'InProgress'");

        await _orderRepository.DeleteAsync(orderId);
    }

    public async Task SetOrderStatusAsync(Guid orderId, string status, DateTime? shipmentDate = null)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new InvalidOperationException($"Order with id {orderId} not found");

        if (!OrderStatuses.All.Contains(status))
            throw new InvalidOperationException($"Invalid order status: {status}");

        order.Status = status;
        if (shipmentDate.HasValue)
            order.ShipmentDate = shipmentDate.Value;

        await _orderRepository.UpdateAsync(order);
    }

    public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId, string? status = null)
    {
        if (string.IsNullOrEmpty(status))
            return await _orderRepository.GetByCustomerIdAsync(customerId);

        return await _orderRepository.GetByCustomerIdAndStatusAsync(customerId, status);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }
}
