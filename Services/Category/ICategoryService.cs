using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface ICategoryService
{
    Task<Category> AddAsync(Category category);

    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category> GetByIdAsync(int id);

    Task UpdateAsync(Category category);

    Task DeleteAsync(int id);
}