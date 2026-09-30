using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class PurchaseService : IPurchaseService
{

    private readonly IPurchaseRepository _purchase;
    public PurchaseService(IPurchaseRepository purchase)
    {
        _purchase = purchase;
    }

    public async Task<Purchase> AddAsync(Purchase purchase)
    {
        var record = await _purchase.AddAsync(purchase);
        if (record is null)
            throw new Exception("this purchase cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("Purchase not found to delete it");
        await _purchase.DeleteAsync(record);
    }

    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        var records = await _purchase.GetAllAsync();
        if (!records.Any())
            throw new Exception("No purchase found try to add one");
        return records; 
    }

    public async Task<Purchase> GetByIdAsync(int id)
    {
        var record = await _purchase.GetByIdAsync(id);
        return record; 
    }

    public async Task UpdateAsync(Purchase purchase)
    {
        var record = await GetByIdAsync(purchase.Id);
        if (record is null)
            throw new Exception("purchase not found to update");
        await _purchase.UpdateAsync(purchase);
    }
}