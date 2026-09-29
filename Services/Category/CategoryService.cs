using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class CategoryService : ICategoryService
{

    private readonly ICategoryRepository _category;
    public CategoryService(ICategoryRepository category)
    {
        _category = category;
    }

    public async Task<Category> AddAsync(Category category)
    {
        var record = await _category.CreateAsync(category);
        if (record is null)
            throw new Exception("this category can`t added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("category not found to delete it");
        await _category.DeleteAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var records = await _category.GetAllAsync();
        if (!records.Any())
            throw new Exception("No categries found try to add one");
        return records;
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        var record = await _category.GetIDAsync(id);
        if (record is null)
            throw new Exception("Category not found");
        return record;   
    }

    public async Task UpdateAsync(Category category)
    {
        var record = await GetByIdAsync(category.Id);
        if (record is null)
            throw new Exception("category not found to update");
        await _category.UpdateAsync(category);   
     }
}