using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Infrastructure.Persistence.Repositories;
using InventoryManagementSystem.Infrastructure.Persistence;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Application.Abstractions.Persistence;
public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);
    }

}

