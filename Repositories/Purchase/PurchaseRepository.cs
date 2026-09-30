using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace InventoryMangmentSystem.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;
    public PurchaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase> AddAsync(Purchase purchase)
    {
        await _context.Purchases.AddAsync(purchase);
        await _context.SaveChangesAsync();
        return purchase;
    }

    public async Task DeleteAsync(Purchase purchase)
    {
        _context.Purchases.Remove(purchase);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        return await _context.Purchases.ToListAsync();
    }

    public async Task<Purchase?> GetByIdAsync(int id)
    {
        return await _context.Purchases.FindAsync(id);
    }
    public async Task<IEnumerable<Purchase>> GetByProductIdAsync(int productId)
    {
        return await _context.Purchases
            .Where(p => p.PurchaseItems
                .Any(item => item.ProductId == productId))
            .ToListAsync();
    }
    public async Task UpdateAsync(Purchase purchase)
    {
        var record = await GetByIdAsync(purchase.Id);
        record.SupplierId = purchase.SupplierId;
        record.Date = purchase.Date;
        record.TotalAmount = purchase.TotalAmount;
        await _context.SaveChangesAsync();
    }   
}