using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetByProductVariantIdAsync(Guid productVariantId, Guid warehouseId);

    Task<Stock?> GetByProductIdAsync(Guid productId, Guid warehouseId);

    Task<IEnumerable<LowStockDto>> GetLowStockAsync(Guid warehouseId);

    Task<IEnumerable<StockResponseDto>> GetAllAsync(List<Guid> warehouseIds);
}
