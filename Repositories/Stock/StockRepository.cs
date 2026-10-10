using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class StockRepository : BaseRepository<Stock>
{
    public StockRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Stock?> GetByProductIdAsync(
        Guid productId,
        Guid warehouseId)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s =>
                s.ProductId == productId &&
                s.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<Stock>> GetLowStockAsync()
    {
        return await _context.Stocks
            .Where(s => s.Quantity <= 5)
            .ToListAsync();
    }

    public async Task<Stock> IncreaseStockAsync(
        Guid productId,
        int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);

        if (stock is null)
            throw new Exception("Stock not found");

        stock.Quantity += quantity;

        return stock;
    }

    public async Task DecreaseStockAsync(
        Guid productId,
        int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);

        if (stock is null)
            throw new Exception("Stock not found");

        stock.Quantity -= quantity;
    }
}