using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryMangmentSystem.Repositories;

public class SaleItemRepository : ISaleItemsRepository
{
    private readonly ApplicationDbContext _context;
    public SaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<SaleItem>> GetAllAsync()
    {
        return await _context.SaleItems.ToListAsync();
    }

    public async Task<SaleItem?> GetByIdAsync(int id)
    {
        return await _context.SaleItems.FindAsync(id);
    }
    public async Task Delete(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("no sales found");
        _context.SaleItems.Remove(record);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(SaleItem saleItem)
    {
        var record = await GetByIdAsync(saleItem.Id);
        if (record is null)
            throw new Exception("no sales found");
        record.Quantity = saleItem.Quantity;
        record.UnitPrice = saleItem.UnitPrice;
        await _context.SaveChangesAsync();
    }

    public async Task<SaleItem> CreateAsync(SaleItem saleItem)
    {
        await _context.SaleItems.AddAsync(saleItem);
        await _context.SaveChangesAsync();
        return saleItem;
    }

    // public async Task<Sale?> GetSaleWithItemsAsync(int id)
    // {
    //     return await _context.Sales
    //         .Include(s => s.SaleItems)
    //         .ThenInclude(si => si.Product)
    //         .FirstOrDefaultAsync(s => s.Id == id);
    // }
}