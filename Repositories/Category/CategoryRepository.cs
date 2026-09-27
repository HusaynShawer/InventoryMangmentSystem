using System.Diagnostics.Eventing.Reader;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem;

public class CategoryRepository : ICategoryRepository
{

    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Category> CreateAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
       
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> DeleteAsync(int id)
    {
        var record = await GetIDAsync(id);
        return record; 
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var records = await _context.Categories.ToListAsync();

        if (records is null)
            throw new Exception("not Categories found");

        return records;
    }

    public async Task<Category> GetIDAsync(int id)
    {
        var record = await _context.Categories.FindAsync(id);
        if (record is null)
            throw new Exception("categor is not found");
        return record;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var record = await GetIDAsync(category.Id);
        if (record is not null)
        {
            record.Name = category.Name;
            await _context.SaveChangesAsync();
        }
        throw new Exception("Category not found to update");
    }
}