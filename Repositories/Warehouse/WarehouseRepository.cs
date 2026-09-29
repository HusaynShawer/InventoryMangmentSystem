using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class WarehouseRepository : IWarehouseRepository
{

    private readonly ApplicationDbContext _context;
    public WarehouseRepository(ApplicationDbContext context)
    {
        _context = context;        
    }
    public async Task<Warehouse> CreateAsync(Warehouse warehouse)
    {
        await _context.Warehouses.AddAsync(warehouse);
        await _context.SaveChangesAsync();
        return warehouse;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
       
        if (record is null)
            throw new Exception("warehouse not found");
       
        _context.Warehouses.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses.ToListAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouses.FindAsync(id);
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        var record = await GetByIdAsync(warehouse.Id);
       
        if (record is null)
            throw new Exception("warehouse not found");
        
        record.Location = warehouse.Location;
        record.Name = warehouse.Name;
        await _context.SaveChangesAsync();
    }
}