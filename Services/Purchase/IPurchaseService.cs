using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface IPurchaseService
{
    Task<Purchase> AddAsync(Purchase purchase);

    Task<IEnumerable<Purchase>> GetAllAsync();

    Task<Purchase> GetByIdAsync(int id);

    Task UpdateAsync(Purchase purchase);

    Task DeleteAsync(int id);
}