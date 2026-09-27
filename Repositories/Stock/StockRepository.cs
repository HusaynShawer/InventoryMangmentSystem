using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem;

public class StockRepository : IStockRepository
{
    private readonly ApplicationDbContext _context;

    public StockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Stock> CreateAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();

        return stock;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _context.Stocks.ToListAsync();
    }

    public async Task<Stock?> GetByProductIdAsync(int productId)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s => s.productId == productId);
    }

    public async Task<IEnumerable<Stock>> GetLowStockAsync()
    {
        return await _context.Stocks
            .Where(s => s.Quantity <= 5)
            .ToListAsync();
    }

    public async Task IncreaseStockAsync(int productId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be bigger than 0");

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.productId == productId);

        if (stock is null)
            throw new Exception("No stock here");

        stock.Quantity += quantity;

        await _context.SaveChangesAsync();
    }

    public async Task DecreaseStockAsync(int productId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be bigger than 0");

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.productId == productId);

        if (stock is null)
            throw new Exception("No stock here");

        if (stock.Quantity < quantity)
            throw new Exception("Not enough stock");

        stock.Quantity -= quantity;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock)
    {
        var record = await _context.Stocks.FindAsync(stock.Id);

        if (record is null)
            throw new Exception("No stock here");

        record.Quantity = stock.Quantity;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.productId == productId);

        if (stock is null)
            throw new Exception("Stock not found");

        _context.Stocks.Remove(stock);

        await _context.SaveChangesAsync();
    }
}