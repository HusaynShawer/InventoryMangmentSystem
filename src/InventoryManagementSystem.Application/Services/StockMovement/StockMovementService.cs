using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
namespace InventoryManagementSystem.Application.Services;

public class StockMovementService
{
    private readonly IRepository<StockMovement> _repo;

    public StockMovementService(
        IRepository<StockMovement> repo)
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
        var recod = await _repo.AddAsync(stockMovement);
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
        record.StockLevelId = stockMovement.StockLevelId;
        record.ProductVariantId = stockMovement.ProductVariantId;
        record.WarehouseId = stockMovement.WarehouseId;
        record.UserId = stockMovement.UserId;
        record.CreatedByUserId = stockMovement.CreatedByUserId;
        record.MovementType = stockMovement.MovementType;
        record.QuantityDelta = stockMovement.QuantityDelta;
        record.ResultingOnHand = stockMovement.ResultingOnHand;
        record.ReferenceType = stockMovement.ReferenceType;
        record.ReferenceId = stockMovement.ReferenceId;
        record.IdempotencyKey = stockMovement.IdempotencyKey;
        record.CreatedAt = stockMovement.CreatedAt;
        record.Note = stockMovement.Note;
        _repo.Update(record);
    }
}