using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Controllers;

public interface IProductController
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}