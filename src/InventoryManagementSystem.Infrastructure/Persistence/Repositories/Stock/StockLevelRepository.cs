using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class StockLevelRepository : BaseRepository<StockLevel>, IStockLevelRepository
{
    public StockLevelRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<StockLevel?> FindAsync(Guid productVariantId, Guid warehouseId)
    {
        return await _context.StockLevels
            .FirstOrDefaultAsync(s =>
                s.ProductVariantId == productVariantId &&
                s.WarehouseId == warehouseId);
    }

    public async Task<StockLevel> GetAsync(Guid productVariantId, Guid warehouseId)
    {
        return await FindAsync(productVariantId, warehouseId)
            ?? throw new NotFoundException("Stock level not found.");
    }

    public async Task<StockLevel> GetOrCreateAsync(Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = await FindAsync(productVariantId, warehouseId);
        if (stockLevel is not null)
            return stockLevel;

        return await CreateAsync(productVariantId, warehouseId);
    }

    public async Task<StockLevel> CreateAsync(Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = new StockLevel
        {
            ProductVariantId = productVariantId,
            WarehouseId = warehouseId,
            QuantityOnHand = 0,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.StockLevels.AddAsync(stockLevel);
        return stockLevel;
    }

    public async Task<IEnumerable<LowStockDto>> GetLowStockAsync(Guid warehouseId)
    {
        return await _context.StockLevels
            .Where(s => s.WarehouseId == warehouseId)
            .Include(s => s.ProductVariant)
                .ThenInclude(v => v.Product)
            .Include(s => s.Warehouse)
            .Where(s => s.QuantityOnHand - s.ReservedQuantity <= s.ProductVariant.Product!.ReorderLevel)
            .Select(s => new LowStockDto
            {
                ProductId = s.ProductVariant.ProductId,
                ProductName = s.ProductVariant.Product!.Name ?? string.Empty,
                Sku = s.ProductVariant.Sku,
                WarehouseId = s.WarehouseId,
                WarehouseName = s.Warehouse.Name,
                Quantity = s.QuantityOnHand - s.ReservedQuantity
            }).ToListAsync();
    }

    public async Task<IEnumerable<StockResponseDto>> GetAllAsync(List<Guid> warehouseIds)
    {
        return await _context.StockLevels
            .Where(s => warehouseIds.Contains(s.WarehouseId))
            .Select(s => new StockResponseDto
            {
                Id = s.Id,
                ProductId = s.ProductVariant.ProductId,
                ProductVariantId = s.ProductVariantId,
                WarehouseId = s.WarehouseId,
                Quantity = s.QuantityOnHand,
                QuantityOnHand = s.QuantityOnHand,
                ReservedQuantity = s.ReservedQuantity,
                AvailableQuantity = s.QuantityOnHand - s.ReservedQuantity
            }).ToListAsync();
    }
}
