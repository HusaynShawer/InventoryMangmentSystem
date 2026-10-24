using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services;

public interface IPurchaseItemService
{
    Task<PurchaseItem> AddAsync(PurchaseItem purchaseItems);

    Task<IEnumerable<PurchaseItem>> GetAllAsync();

    Task<PurchaseItem> GetByIdAsync(Guid id);

    Task UpdateAsync(PurchaseItem purchaseItems);

    Task DeleteAsync(Guid id);
}