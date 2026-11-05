using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.StockLevels)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.StockLevels)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
