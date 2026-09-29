using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Services;

public interface IStockService
{
    Task<Stock> GetByProductIdAsync(int productId);

    Task<IEnumerable<Stock>> GetAllAsync();

    Task<Stock> AddAsync(Stock stock);

    Task UpdateAsync(Stock stock);

    Task DeleteAsync(int id);

    Task IncreaseStockAsync(int productId, int quantity);

    Task DecreaseStockAsync(int productId, int quantity);

    Task<IEnumerable<Stock>> GetLowStockAsync();
}