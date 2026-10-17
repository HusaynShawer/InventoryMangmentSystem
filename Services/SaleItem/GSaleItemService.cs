using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GsaleItemService
{
    private readonly BaseRepository<SaleItem> _repo;

    public GsaleItemService(
        BaseRepository<SaleItem> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SaleItem>> GetAll()
    {
        return await _repo.GetAll();
    }
    public async Task<SaleItem> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
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
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(SaleItem saleItem)
    {
        var record = await GetByID(saleItem.Id);
        record.Quantity = saleItem.Quantity;         
        record.UnitPrice = saleItem.UnitPrice; 
        _repo.Update(record);
    }
}