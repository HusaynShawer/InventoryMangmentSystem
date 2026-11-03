using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();

    Task<Product?> GetByIdWithDetailsAsync(Guid id);
}
