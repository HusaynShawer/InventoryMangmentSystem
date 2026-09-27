using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace InventoryMangmentSystem.Repositories;

public class PurchaseItemRepository : IPurchaseItemRepository
{

    private readonly ApplicationDbContext _context;
    public PurchaseItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PurchaseItems> AddAsync(PurchaseItems purchaseItems)
    {
        await _context.PurchaseItems.AddAsync(purchaseItems);
        await _context.SaveChangesAsync();
        return purchaseItems;
        
    }

    public async Task DeleteAsync(int id)
    {
        var record = await _context.PurchaseItems.FindAsync(id);
        if (record is null)
        {
            throw new Exception("PurchaseItems is not found to delete it");
        }
        _context.PurchaseItems.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PurchaseItems>> GetAllAsync()
    {
        return await _context.PurchaseItems.ToListAsync();
    }

    public async Task<PurchaseItems?> GetByIdAsync(int id)
    {
        return await _context.PurchaseItems.FindAsync(id);
    }

    public async Task<IEnumerable<PurchaseItems>> GetByProductIdAsync(int productId)
    {
        return await _context.PurchaseItems.Where(s=> s.ProductId == productId).ToListAsync();
    }

    public async Task UpdateAsync(PurchaseItems purchaseItems)
    {
        var record = await GetByIdAsync(purchaseItems.Id);
        if (record is null)
            throw new Exception("Purchase item is not found try with another id please");
        record.Quantity = purchaseItems.Quantity;
        record.UnitPrice = purchaseItems.UnitPrice;
        await _context.SaveChangesAsync();
    }
}