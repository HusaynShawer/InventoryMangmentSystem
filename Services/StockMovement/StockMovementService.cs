using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class StockMovementService : IStockMovementService
{

    private readonly IStockMovementRepository _stockMovement;
    public StockMovementService(IStockMovementRepository stockMovement)
    {
        _stockMovement = stockMovement;
    }

    public async Task<StockMovement> AddAsync(StockMovement stockMovement)
    {
        var record = await _stockMovement.CreateAsync(stockMovement);
        if (record is null)
            throw new Exception("this StockMovement cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        await _stockMovement.DeleteAsync(record);
    }

    public async Task<IEnumerable<StockMovement>> GetAllAsync()
    {
        var records = await _stockMovement.GetAllAsync();
        if (!records.Any())
            throw new Exception("No StockMovements found try to add one");
        return records; 
    }

    public async Task<StockMovement> GetByIdAsync(int id)
    {
        var record = await _stockMovement.GetByIdAsync(id);
        if (record is null)
            throw new Exception("StockMovement not found");
        return record; 
    }

    public async Task UpdateAsync(StockMovement stockMovement)
    {
        var record = await GetByIdAsync(stockMovement.Id);
        if (record is null)
            throw new Exception("StockMovement not found to update");
        await _stockMovement.UpdateAsync(stockMovement);
    }
}