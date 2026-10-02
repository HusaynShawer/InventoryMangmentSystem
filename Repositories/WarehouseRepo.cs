using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class WarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetWarehouseProduct(int userId)
    {
        var records = await _context.Products
            .Where(p => p.Stocks.Any(s =>
                s.Warehouse.UserWarehouses
                    .Any(uwh => uwh.UserId == userId)))
            .ToListAsync();

        return records;
    }
}