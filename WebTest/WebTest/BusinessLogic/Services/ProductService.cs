using WebTest.DataAccess.Repositories;
using WebTest.Models;
using WebTest.ViewModels.DTOs.Requests;

namespace WebTest.BusinessLogic.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        var codeNum = await _productRepository.GetCountAsync() + 1;
        var code = GenerateProductCode(codeNum);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Description
        };

        return await _productRepository.AddAsync(product);
    }

    public async Task<Product> UpdateProductAsync(EditProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id);
        if (product == null)
            throw new InvalidOperationException($"Product with id {dto.Id} not found");

        product.Name = dto.Name;
        product.Category = dto.Description;
        product.Price = dto.Price;

        await _productRepository.UpdateAsync(product);
        return product;
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public string GenerateProductCode(int sequentialNumber)
    {
        return $"{sequentialNumber:D2}-{sequentialNumber:D4}-AA00";
    }
}
