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

    public async Task<Stock?> GetByProductIdAsync(Guid productId, Guid warehouseId)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s =>
                s.ProductId == productId &&
                s.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<LowStockDto>> GetLowStockAsync(Guid warehouseId)
    {
        return await _context.Stocks.Where(s => s.WarehouseId == warehouseId &&
                                        s.Quantity <= s.Product.ReorderLevel).Select(s => new LowStockDto
                                        {
                                            ProductId = s.ProductId,
                                            ProductName = s.Product.Name ?? string.Empty,
                                            Sku = s.Product.Sku ?? string.Empty,
                                            WarehouseId = s.WarehouseId,
                                            WarehouseName = s.Warehouse.Name,
                                            Quantity = s.Quantity
                                        }).ToListAsync();
    }

    public async Task<Stock> IncreaseStockAsync(Guid productId, int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);

        if (stock is null)
            throw new NotFoundException("Stock not found.");

        stock.Quantity += quantity;

        return stock;
    }

    public async Task DecreaseStockAsync(Guid productId, int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);

        if (stock is null)
            throw new NotFoundException("Stock not found.");

        stock.Quantity -= quantity;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await base.GetAllAsync();
    }

    public async Task<Stock?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }
}
