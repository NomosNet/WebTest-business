using WebTest.Models;
using WebTest.ViewModels.DTOs.Requests;

namespace WebTest.BusinessLogic.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(CreateProductDto dto);
    Task<Product> UpdateProductAsync(EditProductDto dto);
    Task DeleteProductAsync(Guid id);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<List<Product>> GetAllProductsAsync();
    string GenerateProductCode(int sequentialNumber);
}
