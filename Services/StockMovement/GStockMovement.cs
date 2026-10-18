using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GStockMovementService
{
    private readonly BaseRepository<StockMovement> _repo;

    public GStockMovementService(
        BaseRepository<StockMovement> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<StockMovement>> GetAll()
    {
        return await _repo.GetAll();
    }
    public async Task<StockMovement> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
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
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(StockMovement stockMovement)
    {
        var record = await GetByID(stockMovement.Id);
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