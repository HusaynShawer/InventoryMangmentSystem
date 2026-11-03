using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class StockLevelRepository : BaseRepository<StockLevel>, IStockLevelRepository
{
    public StockLevelRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<StockLevel> GetOrCreateAsync(Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(s =>
                s.ProductVariantId == productVariantId &&
                s.WarehouseId == warehouseId);

        if (stockLevel is not null)
            return stockLevel;

        stockLevel = new StockLevel
        {
            Id = Guid.NewGuid(),
            ProductVariantId = productVariantId,
            WarehouseId = warehouseId,
            QuantityOnHand = 0,
            ReservedQuantity = 0,
            AvailableQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.StockLevels.AddAsync(stockLevel);
        return stockLevel;
    }

    public async Task<StockLevel> GetOrCreateForStockAsync(Stock stock)
    {
        var stockLevel = await GetOrCreateAsync(stock.ProductVariantId, stock.WarehouseId);

        stockLevel.QuantityOnHand = stock.Quantity;
        stockLevel.AvailableQuantity = stock.Quantity - stockLevel.ReservedQuantity;
        stockLevel.UpdatedAt = DateTime.UtcNow;

        return stockLevel;
    }
}
