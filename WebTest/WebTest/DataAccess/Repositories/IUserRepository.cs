using WebTest.Models;

namespace WebTest.DataAccess.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser?> GetByUserNameAsync(string userName);
    Task<AppUser?> GetByUserNameAndPasswordAsync(string userName, string password);
    Task<List<AppUser>> GetAllAsync();
    Task<AppUser> AddAsync(AppUser user);
    Task UpdateAsync(AppUser user);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
