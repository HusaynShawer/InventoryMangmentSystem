using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IStockLevelRepository : IRepository<StockLevel>
{
    Task<StockLevel> GetOrCreateAsync(Guid productVariantId, Guid warehouseId);

    Task<StockLevel> GetOrCreateForStockAsync(Stock stock);
}
