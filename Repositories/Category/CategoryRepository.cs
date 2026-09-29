using System.Diagnostics.Eventing.Reader;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

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

    public async Task DeleteAsync(int id)
    {
        var record = await GetIDAsync(id);
        _context.Categories.Remove(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();

    }

    public async Task<Category?> GetIDAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task UpdateAsync(Category category)
    {
        var record = await GetIDAsync(category.Id);
        record.Name = category.Name;
        await _context.SaveChangesAsync();
    }
}