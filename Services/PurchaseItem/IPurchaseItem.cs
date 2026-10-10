using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface IPurchaseItemsService
{
    Task<PurchaseItems> AddAsync(PurchaseItems purchaseItems);

    Task<IEnumerable<PurchaseItems>> GetAllAsync();

    Task<PurchaseItems> GetByIdAsync(Guid id);

    Task UpdateAsync(PurchaseItems purchaseItems);

    Task DeleteAsync(Guid id);
}