using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface IStockRepository
{
    Task<Stock?> GetByProductIdAsync(Guid productId);

    Task<IEnumerable<Stock>> GetAllAsync();

    Task<Stock> CreateAsync(Stock stock);

    Task UpdateAsync(Stock stock);

    Task DeleteAsync(Stock stock);

    Task IncreaseStockAsync(Guid productId, int quantity);

    Task DecreaseStockAsync(Guid productId, int quantity);

    Task<IEnumerable<Stock>> GetLowStockAsync();
}