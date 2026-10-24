using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
namespace InventoryManagementSystem.Services;

public class StockMovementService
{
    private readonly BaseRepository<StockMovement> _repo;

    public StockMovementService(
        BaseRepository<StockMovement> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<StockMovement>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<StockMovement> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Stock movement not found.");
        return record;
    }
    public async Task<StockMovement> Create(StockMovement stockMovement)
    {
        var recod = await _repo.Add(stockMovement);
        if (recod is null)
            throw new InvalidOperationException(
                "Stock movement could not be created.");
        return stockMovement;

    }


    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
    }
    public async Task Update(StockMovement stockMovement)
    {
        var record = await GetByIdAsync(stockMovement.Id);
        record.ProductId = stockMovement.ProductId;
        record.WarehouseId = stockMovement.WarehouseId;
        record.UserId = stockMovement.UserId;
        record.Quantity = stockMovement.Quantity;
        record.MovementType = stockMovement.MovementType;
        record.Date = stockMovement.Date;
        record.Note = stockMovement.Note;
        _repo.Update(record);
    }
}