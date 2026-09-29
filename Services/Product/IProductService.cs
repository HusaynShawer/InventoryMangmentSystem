using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface IProductService
{
    Task<Product> AddAsync(Product product);

    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product> GetByIdAsync(int id);

    Task UpdateAsync(Product product);

    Task DeleteAsync(int id);
}