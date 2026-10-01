using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryMangmentSystem.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;
    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Sale>> GetAllSalesAsync()
    {
        return await _context.Sales.ToListAsync();
    }

    public async Task<Sale?> GetSaleAsync(int id)
    {
        return await _context.Sales.FindAsync(id);
    }
    public async Task Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(Sale sale)
    {
        var record = await GetSaleAsync(sale.Id);
        if (record is null)
            throw new Exception("Sale not found");

        record.WarehouseId = sale.WarehouseId;         
        record.CreatedByUserId = sale.CreatedByUserId; 
        record.TotalAmount = sale.TotalAmount;

        await _context.SaveChangesAsync();
    }

    public async Task<Sale> CreateAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
        return sale;
    }

    // public async Task<Sale?> GetSaleWithItemsAsync(int id)
    // {
    //     return await _context.Sales
    //         .Include(s => s.SaleItems)
    //         .ThenInclude(si => si.Product)
    //         .FirstOrDefaultAsync(s => s.Id == id);
    // }
}