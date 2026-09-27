using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface IPurchaseItemRepository
{
    Task<PurchaseItems> AddAsync(PurchaseItems purchaseItems);

    Task<IEnumerable<PurchaseItems>> GetAllAsync();

    Task<PurchaseItems?> GetByIdAsync(int id);

   Task<IEnumerable<PurchaseItems>> GetByProductIdAsync(int productId);

    Task UpdateAsync(PurchaseItems purchaseItems);

    Task DeleteAsync(int id);
}