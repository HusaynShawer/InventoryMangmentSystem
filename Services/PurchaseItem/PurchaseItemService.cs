using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class PurchaseItemsService : IPurchaseItemsService
{

    private readonly IPurchaseItemRepository _purchaseItem;
    public PurchaseItemsService(IPurchaseItemRepository purchaseItems)
    {
        _purchaseItem = purchaseItems;
    }

    public async Task<PurchaseItems> AddAsync(PurchaseItems purchaseItems)
    {
        var record = await _purchaseItem.AddAsync(purchaseItems);
        if (record is null)
            throw new Exception("this purchaseItem cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("purchaseItem not found to delete it");
        await _purchaseItem.DeleteAsync(id);
    }

    public async Task<IEnumerable<PurchaseItems>> GetAllAsync()
    {
        var records = await _purchaseItem.GetAllAsync();
        if (!records.Any())
            throw new Exception("No purchaseItem found try to add one");
        return records; 
    }

    public async Task<PurchaseItems> GetByIdAsync(int id)
    {
        var record = await _purchaseItem.GetByIdAsync(id);
        if (record is null)
            throw new Exception("purchaseItem not found");
        return record; 
    }

    public async Task UpdateAsync(PurchaseItems purchaseItems)
    {
        var record = await GetByIdAsync(purchaseItems.Id);
        if (record is null)
            throw new Exception("purchaseItem not found to update");
        await _purchaseItem.UpdateAsync(purchaseItems);
    }
}