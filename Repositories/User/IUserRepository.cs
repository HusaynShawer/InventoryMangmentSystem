using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;
public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}