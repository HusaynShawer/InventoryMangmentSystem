using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class SaleItemService : ISaleItemService
{

    private readonly ISaleItemRepository _saleItem;
    public SaleItemService(ISaleItemRepository saleItem)
    {
        _saleItem = saleItem;
    }

    public async Task<SaleItem> AddAsync(SaleItem saleItem)
    {
        var record = await _saleItem.CreateAsync(saleItem);
        if (record is null)
            throw new Exception("this SaleItem cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("SaleItem not found to delete it");
        await _saleItem.Delete(record);
    }

    public async Task<IEnumerable<SaleItem>> GetAllAsync()
    {
        var records = await _saleItem.GetAllAsync();
        if (!records.Any())
            throw new Exception("No SaleItems found try to add one");
        return records; 
    }

    public async Task<SaleItem> GetByIdAsync(int id)
    {
        var record = await _saleItem.GetByIdAsync(id);
        if (record is null)
            throw new Exception("SaleItem not found");
        return record; 
    }

    public async Task UpdateAsync(SaleItem saleItem)
    {
        var record = await GetByIdAsync(saleItem.Id);
        if (record is null)
            throw new Exception("SaleItem not found to update");
        await _saleItem.UpdateAsync(saleItem);
    }
}