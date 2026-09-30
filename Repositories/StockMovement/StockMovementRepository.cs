using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class StockMovementRepository : IStockMovementRepository
{

    private readonly ApplicationDbContext _context;
    public StockMovementRepository(ApplicationDbContext context)
    {
        _context = context;
        
    }
    public async Task<StockMovement> CreateAsync(StockMovement stockMovement)
    {
        await _context.StockMovements.AddAsync(stockMovement);
        await _context.SaveChangesAsync();

        return stockMovement;
    }

    public async Task DeleteAsync(StockMovement stockMovement)
    {
        _context.StockMovements.Remove(stockMovement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockMovement>> GetAllAsync()
    {
        return await _context.StockMovements.ToListAsync();

    }

    public async Task<StockMovement?> GetByIdAsync(int id)
    {
        return await _context.StockMovements.FindAsync(id);
    }

    public async Task UpdateAsync(StockMovement stockMovement)
    {
        var record = await _context.StockMovements.FindAsync(stockMovement.Id);

        if (record is null)
            throw new Exception("No stock here");

        record.Warehoused = stockMovement.Warehoused;
        record.Quantity = stockMovement.Quantity;
        record.MovementType = stockMovement.MovementType;
        record.Date = stockMovement.Date;
        record.Note = stockMovement.Note;

        await _context.SaveChangesAsync();
    }
}