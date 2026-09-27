using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface IPurchaseRepository
{
    Task<Purchase> AddAsync(Purchase purchase);

    Task<IEnumerable<Purchase>> GetAllAsync();

    Task<Purchase?> GetByIdAsync(int id);

   Task<IEnumerable<Purchase>> GetByProductIdAsync(int productId);

    Task UpdateAsync(Purchase purchase);

    Task DeleteAsync(int id);
}