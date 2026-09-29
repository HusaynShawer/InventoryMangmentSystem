using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class ProductRepository : IProductRepository
{

    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await _context.Products.FindAsync(id);
        _context.Products.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
            
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);     
    }

    public async Task UpdateAsync(Product product)
    {
        var record = await _context.Products.FindAsync(product.Id);
        record.Name = product.Name;
        record.Sku = product.Sku;
        record.UnitPrice = product.UnitPrice;
        await _context.SaveChangesAsync();
    }
    
}