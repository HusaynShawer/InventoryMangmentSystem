using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;
public interface ICartRepository : IRepository<Cart>
{
    public Task<Cart?> GetByUserIdAsync(Guid userId);
}