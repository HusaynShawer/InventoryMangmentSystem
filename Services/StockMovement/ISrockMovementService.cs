using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface IStockMovementService
{
    Task<StockMovement> AddAsync(StockMovement stockMovement);

    Task<IEnumerable<StockMovement>> GetAllAsync();

    Task<StockMovement> GetByIdAsync(int id);

    Task UpdateAsync(StockMovement stockMovement);

    Task DeleteAsync(int id);
}