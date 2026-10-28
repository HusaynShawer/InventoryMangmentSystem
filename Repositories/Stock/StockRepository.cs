using InventoryManagementSystem.Data;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repositories;

public class StockRepository : BaseRepository<Stock>
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

    public async Task<Stock?> GetByProductIdAsync(Guid productVariantId, Guid warehouseId)
    {
        return await GetByProductVariantIdAsync(productVariantId, warehouseId);
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

    public async Task<Stock> IncreaseStockAsync(Guid productVariantId, int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductVariantId == productVariantId);

        if (stock is null)
            throw new NotFoundException("Stock not found.");

        stock.Quantity += quantity;

        return stock;
    }

    public async Task DecreaseStockAsync(Guid productVariantId, int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductVariantId == productVariantId);

        if (stock is null)
            throw new NotFoundException("Stock not found.");

        stock.Quantity -= quantity;
    }

    public async Task<IEnumerable<StockResponseDto>> GetAllAsync(List<Guid> warehouseIds)
    {

        return await _context.Stocks.Where(s =>warehouseIds.Contains(s.WarehouseId)).Select(s => new StockResponseDto
        {
            Id = s.Id,
            WarehouseId = s.WarehouseId,
            Quantity=s.Quantity
        }).ToListAsync();
    }

    public async Task<Stock?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }
}
