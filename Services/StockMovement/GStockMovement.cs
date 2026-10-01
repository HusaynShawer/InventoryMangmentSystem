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
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No StockMovements found");
        return recods;
    }
    public async Task<StockMovement> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("StockMovement Not found");
        return record;
    }
    public async Task<StockMovement> Create(StockMovement stockMovement)
    {
        var recod = await _repo.Add(stockMovement);
        if (recod is null)
            throw new Exception("sorry StockMovement item dont added try again");
        return stockMovement;

    }


    public async Task Delete(int id)
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