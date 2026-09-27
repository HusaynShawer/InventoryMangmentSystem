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
        
        if(record is null)
            return;
        
        _context.Products.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var records = await _context.Products.ToListAsync();
        
        if (records is null)
            throw new Exception("no products found");

        return records;    
            
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var records = await _context.Products.FindAsync(id);
        
        if (records is null)
            throw new Exception("prodcut not found");
            
        return records;      
    }

    public async Task UpdateAsync(Product product)
    {
        var record = await _context.Products.FindAsync(product.Id);

        if (record is not null)
        {
            record.Name = product.Name;
            record.Sku = product.Sku;
            record.UnitPrice = product.UnitPrice;
            await _context.SaveChangesAsync();
        }
    }
    
}