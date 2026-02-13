using WebTest.Models;

namespace WebTest.BusinessLogic.Services;

public interface ICustomerService
{
    Task<Customer> GetOrCreateCustomerForUserAsync(int userId, string userName);
    string GenerateCustomerCode(int sequentialNumber, int year);
}
