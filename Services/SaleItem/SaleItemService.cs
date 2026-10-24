using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryManagementSystem.Services;

public class SaleItemService
{
    private readonly BaseRepository<SaleItem> _repo;

    public SaleItemService(
        BaseRepository<SaleItem> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SaleItem>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<SaleItem> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Sale item not found.");
        return record;
    }
    public async Task<SaleItem> Create(SaleItem saleItem)
    {
        var recod = await _repo.Add(saleItem);
        if (recod is null)
            throw new InvalidOperationException(
                "Sale item could not be created.");
        return saleItem;

    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
    }
    public async Task Update(SaleItem saleItem)
    {
        var record = await GetByIdAsync(saleItem.Id);
        record.Quantity = saleItem.Quantity;         
        record.UnitPrice = saleItem.UnitPrice; 
        _repo.Update(record);
    }
}