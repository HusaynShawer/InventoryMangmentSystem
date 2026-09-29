using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class WarehouseService : IWarehouseService
{

    private readonly IWarehouseRepository _warehouse;
    public WarehouseService(IWarehouseRepository warehouse)
    {
        _warehouse = warehouse;
    }

    public async Task<Warehouse> AddAsync(Warehouse warehouse)
    {
        var record = await _warehouse.CreateAsync(warehouse);
        if (record is null)
            throw new Exception("this warehouse cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("warehouse not found to delete it");
        await _warehouse.DeleteAsync(id);
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        var records = await _warehouse.GetAllAsync();
        if (!records.Any())
            throw new Exception("No warehouses found try to add one");
        return records; 
    }

    public async Task<Warehouse> GetByIdAsync(int id)
    {
        var record = await _warehouse.GetByIdAsync(id);
        if (record is null)
            throw new Exception("warehouse not found");
        return record; 
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        var record = await GetByIdAsync(warehouse.Id);
        if (record is null)
            throw new Exception("warehouse not found to update");
        await _warehouse.UpdateAsync(warehouse);
    }
}