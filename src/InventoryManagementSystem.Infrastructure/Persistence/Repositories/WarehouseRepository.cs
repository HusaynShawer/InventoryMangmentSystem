using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Guid>> GetByUserId(Guid userId)
    {
        return await _context.UserWarehouses.Where(uw => uw.UserId == userId).Select(uw => uw.WarehouseId).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetWarehouseProduct(Guid userId)
    {
        var warehouseIds = await GetByUserId(userId);
        return await _context.Stocks
            .Where(s => warehouseIds.Contains(s.WarehouseId) && s.ProductVariant != null)
            .Include(s => s.ProductVariant)!
                .ThenInclude(pv => pv!.Product)
            .Select(s => s.ProductVariant!.Product!)
            .Where(p => p != null)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetWarehouseLowProducts(Guid userId)
    {
        var warehouseIds = await GetByUserId(userId);
        return await _context.Stocks
            .Where(s => warehouseIds.Contains(s.WarehouseId) && s.ProductVariant != null && s.Quantity < 5)
            .Include(s => s.ProductVariant)!
                .ThenInclude(pv => pv!.Product)
            .Select(s => s.ProductVariant!.Product!)
            .Where(p => p != null)
            .Distinct()
            .ToListAsync();
    }
}