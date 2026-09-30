using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface IStockMovementRepository
{
    Task<StockMovement?> GetByIdAsync(int id);

    Task<IEnumerable<StockMovement>> GetAllAsync();

    Task<StockMovement> CreateAsync(StockMovement stockMovement);

    Task UpdateAsync(StockMovement stockMovement);

    Task DeleteAsync(StockMovement stockMovement);

}