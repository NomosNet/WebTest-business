using WebTest.Models;

namespace WebTest.DataAccess.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetAllAsync();
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<List<Order>> GetByCustomerIdAndStatusAsync(Guid customerId, string? status);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int?> GetMaxOrderNumberAsync();
    Task<List<Order>> GetAllWithIncludesAsync();
}
