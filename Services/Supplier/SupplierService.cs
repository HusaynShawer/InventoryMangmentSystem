using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class SupplierService : ISupplierService
{

    private readonly ISupplierRepository _supplier;
    public SupplierService(ISupplierRepository supplier)
    {
        _supplier = supplier;
    }

    public async Task<Supplier> AddAsync(Supplier supplier)
    {
        var record = await _supplier.CreateAsync(supplier);
        if (record is null)
            throw new Exception("this supplier cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        await _supplier.DeleteAsync(record);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        var records = await _supplier.GetAllAsync();
        if (!records.Any())
            throw new Exception("No suppliers found try to add one");
        return records; 
    }

    public async Task<Supplier> GetByIdAsync(int id)
    {
        var record = await _supplier.GetByIdAsync(id);
        if (record is null)
            throw new Exception("supplier not found");
        return record; 
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        var record = await GetByIdAsync(supplier.Id);
        if (record is null)
            throw new Exception("supplier not found to update");
        await _supplier.UpdateAsync(supplier);
    }
}