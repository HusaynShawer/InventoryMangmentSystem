using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repositories;

public class ProductAttributeRepository
{
    private readonly ApplicationDbContext _context;

    public ProductAttributeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductAttribute>> GetAllAsync()
    {
        return await _context.ProductAttributes
            .Include(a => a.Values)
            .ToListAsync();
    }

    public async Task<ProductAttribute?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAttributes
            .Include(a => a.Values)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ProductAttribute?> GetByCodeAsync(string code)
    {
        return await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<ProductAttribute> AddAsync(ProductAttribute attribute)
    {
        await _context.ProductAttributes.AddAsync(attribute);
        return attribute;
    }

    public void Update(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
    }

    public void Delete(ProductAttribute attribute)
    {
        _context.ProductAttributes.Remove(attribute);
    }
}

public class ProductAttributeValueRepository
{
    private readonly ApplicationDbContext _context;

    public ProductAttributeValueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductAttributeValue?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAttributeValues
            .Include(v => v.ProductAttribute)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<ProductAttributeValue> AddAsync(ProductAttributeValue value)
    {
        await _context.ProductAttributeValues.AddAsync(value);
        return value;
    }

    public void Update(ProductAttributeValue value)
    {
        _context.ProductAttributeValues.Update(value);
    }

    public void Delete(ProductAttributeValue value)
    {
        _context.ProductAttributeValues.Remove(value);
    }
}
