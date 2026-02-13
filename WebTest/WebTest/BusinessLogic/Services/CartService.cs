using Microsoft.AspNetCore.Http;
using WebTest.DataAccess.Repositories;
using WebTest.Models;

namespace WebTest.BusinessLogic.Services;

public class CartService : ICartService
{
    private const string CartKey = "cart";
    private readonly IProductRepository _productRepository;

    public CartService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Dictionary<Guid, int> GetCart(ISession session)
    {
        var cart = session.GetString(CartKey);
        return string.IsNullOrEmpty(cart)
            ? new Dictionary<Guid, int>()
            : System.Text.Json.JsonSerializer.Deserialize<Dictionary<Guid, int>>(cart) ?? new Dictionary<Guid, int>();
    }

    public void AddToCart(ISession session, Guid productId)
    {
        var dict = GetCart(session);

        if (dict.ContainsKey(productId))
            dict[productId]++;
        else
            dict[productId] = 1;

        session.SetString(CartKey, System.Text.Json.JsonSerializer.Serialize(dict));
    }

    public void ClearCart(ISession session)
    {
        session.Remove(CartKey);
    }

    public async Task<List<(Product product, int quantity)>> GetCartItemsAsync(ISession session)
    {
        var dict = GetCart(session);
        var ids = dict.Keys.ToList();
        var products = await _productRepository.GetByIdsAsync(ids);

        return products.Select(p => (p, dict[p.Id])).ToList();
    }
}
