using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using System.Security.Authentication;
namespace InventoryManagementSystem.Application.Services;

public class SaleItemService
{
    private readonly IRepository<SaleItem> _repo;

    public SaleItemService(
        IRepository<SaleItem> repo)
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
        var recod = await _repo.AddAsync(saleItem);
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