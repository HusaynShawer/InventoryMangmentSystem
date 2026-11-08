using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;

namespace InventoryManagementSystem.Application.Services;

public class StockMovementService
{
    private readonly IRepository<StockMovement> _repo;
    private readonly IStockLevelRepository _stockLevelRepo;
    private readonly IUnitOfWork _unitOfWork;

    public StockMovementService(
        IRepository<StockMovement> repo,
        IStockLevelRepository stockLevelRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockLevelRepo = stockLevelRepo;
        _unitOfWork = unitOfWork;
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

    public async Task<StockMovement> Create(Guid userId, CreateStockMovementDto dto)
    {
        if (dto.QuantityDelta == 0)
            throw new ArgumentException("Quantity delta cannot be zero.");

        var stockLevel = await _stockLevelRepo.GetOrCreateAsync(
            dto.ProductVariantId, dto.WarehouseId);

        if (dto.QuantityDelta > 0)
            stockLevel.IncreaseOnHand(dto.QuantityDelta);
        else
            stockLevel.DecreaseOnHand(-dto.QuantityDelta);

        var movement = new StockMovement
        {
            StockLevelId = stockLevel.Id,
            ProductVariantId = dto.ProductVariantId,
            WarehouseId = dto.WarehouseId,
            UserId = userId,
            MovementType = dto.MovementType,
            QuantityDelta = dto.QuantityDelta,
            ResultingOnHand = stockLevel.QuantityOnHand,
            ReferenceType = "Manual",
            CreatedAt = DateTime.UtcNow,
            Note = dto.Note
        };

        await _repo.AddAsync(movement);
        await _unitOfWork.SaveAsync();
        return movement;
    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
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
        await _unitOfWork.SaveAsync();
    }
}
