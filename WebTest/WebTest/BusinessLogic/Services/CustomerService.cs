using WebTest.DataAccess.Repositories;
using WebTest.Models;

namespace WebTest.BusinessLogic.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> GetOrCreateCustomerForUserAsync(int userId, string userName)
    {
        var customer = await _customerRepository.GetByAppUserIdAsync(userId);
        if (customer != null)
            return customer;

        var year = DateTime.UtcNow.Year;
        var codeNum = await _customerRepository.GetCountAsync() + 1;
        var code = GenerateCustomerCode(codeNum, year);

        customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = userName,
            Code = code,
            AppUserId = userId
        };

        return await _customerRepository.AddAsync(customer);
    }

    public string GenerateCustomerCode(int sequentialNumber, int year)
    {
        return $"{sequentialNumber:D4}-{year}";
    }
}
