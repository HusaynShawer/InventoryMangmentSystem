using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class StockRepository : BaseRepository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Stock?> GetByProductVariantIdAsync(Guid productVariantId, Guid warehouseId)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s =>
                s.ProductVariantId == productVariantId &&
                s.WarehouseId == warehouseId);
    }

    public async Task<Stock?> GetByProductIdAsync(Guid productId, Guid warehouseId)
    {
        return await GetByProductVariantIdAsync(productId, warehouseId);
    }

    public async Task<IEnumerable<LowStockDto>> GetLowStockAsync(Guid warehouseId)
    {
        return await _context.Stocks
            .Where(s => s.WarehouseId == warehouseId)
            .Include(s => s.ProductVariant)!
                .ThenInclude(pv => pv!.Product)
            .Where(s => s.Quantity <= s.ProductVariant!.Product!.ReorderLevel)
            .Select(s => new LowStockDto
            {
                ProductId = s.ProductVariant!.Product!.Id,
                ProductName = s.ProductVariant!.Product!.Name ?? string.Empty,
                Sku = s.ProductVariant!.Sku ?? s.ProductVariant!.Product!.Sku ?? string.Empty,
                WarehouseId = s.WarehouseId,
                Quantity = s.Quantity
            }).ToListAsync();
    }

    public async Task<IEnumerable<StockResponseDto>> GetAllAsync(List<Guid> warehouseIds)
    {
        return await _context.Stocks
            .Where(s => warehouseIds.Contains(s.WarehouseId))
            .Select(s => new StockResponseDto
            {
                Id = s.Id,
                WarehouseId = s.WarehouseId,
                Quantity = s.Quantity
            }).ToListAsync();
    }
}
