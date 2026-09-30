using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;
    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;        
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task DeleteAsync(Supplier supplier)
    {
        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers.ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers.FindAsync(id);
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        var record = await GetByIdAsync(supplier.Id);
       
        if (record is null)
            throw new Exception("warehouse not found");
        
        record.Name = supplier.Name;
        await _context.SaveChangesAsync();    }
}