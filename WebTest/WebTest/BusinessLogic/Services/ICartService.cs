using WebTest.Models;

namespace WebTest.BusinessLogic.Services;

public interface ICartService
{
    Dictionary<Guid, int> GetCart(ISession session);
    void AddToCart(ISession session, Guid productId);
    void ClearCart(ISession session);
    Task<List<(Product product, int quantity)>> GetCartItemsAsync(ISession session);
}
