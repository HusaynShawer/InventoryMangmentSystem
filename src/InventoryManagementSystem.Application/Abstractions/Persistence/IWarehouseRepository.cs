using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IEnumerable<Guid>> GetByUserId(Guid userId);

    Task<IEnumerable<Product>> GetWarehouseProduct(Guid userId);

    Task<IEnumerable<Product>> GetWarehouseLowProducts(Guid userId);
}
