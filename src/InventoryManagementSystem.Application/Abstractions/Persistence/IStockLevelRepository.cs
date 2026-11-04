using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IStockLevelRepository : IRepository<StockLevel>
{
    Task<StockLevel> GetAsync(Guid productVariantId, Guid warehouseId);

    Task<StockLevel> CreateAsync(Guid productVariantId, Guid warehouseId);
}
