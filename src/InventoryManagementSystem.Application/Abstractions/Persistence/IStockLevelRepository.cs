using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IStockLevelRepository : IRepository<StockLevel>
{
    Task<StockLevel?> FindAsync(Guid productVariantId, Guid warehouseId);

    Task<StockLevel> GetAsync(Guid productVariantId, Guid warehouseId);

    Task<StockLevel> GetOrCreateAsync(Guid productVariantId, Guid warehouseId);

    Task<StockLevel> CreateAsync(Guid productVariantId, Guid warehouseId);

    Task<IEnumerable<LowStockDto>> GetLowStockAsync(Guid warehouseId);

    Task<IEnumerable<StockResponseDto>> GetAllAsync(List<Guid> warehouseIds);
}
