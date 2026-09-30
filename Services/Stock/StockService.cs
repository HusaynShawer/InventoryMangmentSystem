using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class StockService : IStockService
{

    private readonly IStockRepository _stock;
    public StockService(IStockRepository stock)
    {
        _stock = stock;
    }
    public async Task<Stock> AddAsync(Stock stock)
    {
        var record = await _stock.CreateAsync(stock);
        if(record is null)
            throw new Exception("Stock never added");
        return record;
    }

    public async Task DecreaseStockAsync(int productId, int quantity)
    {
        if (quantity <=0)
            throw new Exception("Quantity must be bigger than 0");
        var record = await GetByProductIdAsync(productId);
        if(record.Quantity < quantity)
            throw new Exception("quntity not enough to decrease ");
        await _stock.DecreaseStockAsync(productId,quantity);
    }

    public async Task DeleteAsync(int productId)
    {
        var record = await GetByProductIdAsync(productId);
        await _stock.DeleteAsync(record);
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _stock.GetAllAsync();
    }

    public async Task<Stock> GetByProductIdAsync(int productId)
    {
        var record = await _stock.GetByProductIdAsync(productId);
        if (record is null)
            throw new Exception("stock of these product not found try with another id please");
        return record;
    }

    public async Task<IEnumerable<Stock>> GetLowStockAsync()
    {
        return await _stock.GetLowStockAsync();
    }

    public async Task IncreaseStockAsync(int productId, int quantity)
    {
        if (quantity <=0)
            throw new Exception("Quantity must be bigger than 0");
        await GetByProductIdAsync(productId);
        await _stock.IncreaseStockAsync(productId,quantity);

    }

    public async Task UpdateAsync(Stock stock)
    {
        await GetByProductIdAsync(stock.productId);
        await _stock.UpdateAsync(stock);
    }
}