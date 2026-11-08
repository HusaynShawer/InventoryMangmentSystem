using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Application.Abstractions.Persistence;

namespace InventoryManagementSystem.Application.Services;

public class StockService
{
    private readonly IStockLevelRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IRepository<StockMovement> _stockMovementRepo;
    private readonly IWarehouseRepository _warehosueRepo;
    private readonly IUnitOfWork _uow;

    public StockService(
        IStockLevelRepository repo,
        IUserRepository userRepo,
        IRepository<StockMovement> stockMovementRepo,
        IWarehouseRepository warehouseRepo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _userRepo = userRepo;
        _stockMovementRepo = stockMovementRepo;
        _warehosueRepo = warehouseRepo;
        _uow = uow;
    }

    public async Task<StockLevel> Create(StockDto dto)
    {
        if (dto.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        var productVariantId = dto.ProductVariantId != Guid.Empty
            ? dto.ProductVariantId
            : dto.ProductId;

        var existing = await _repo.FindAsync(productVariantId, dto.WarehouseId);
        if (existing is not null)
            throw new ConflictException(
                "A stock level already exists for this product variant and warehouse.");

        var stockLevel = new StockLevel
        {
            ProductVariantId = productVariantId,
            WarehouseId = dto.WarehouseId,
            QuantityOnHand = dto.Quantity,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(stockLevel);
        await _uow.SaveAsync();
        return stockLevel;
    }

    public async Task<IEnumerable<StockResponseDto>> GetAllAsync(Guid userId)
    {
        var userWarehousesIds = await _warehosueRepo.GetByUserId(userId);
        return await _repo.GetAllAsync(userWarehousesIds.ToList());
    }

    public async Task<StockLevel> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Stock level not found.");
        return record;
    }

    public async Task<StockLevel> GetStockLevelAsync(
        Guid productVariantId, Guid warehouseId)
    {
        return await _repo.GetAsync(productVariantId, warehouseId);
    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
        await _uow.SaveAsync();
    }

    public async Task Update(Guid id, StockDto dto)
    {
        var record = await GetByIdAsync(id);
        record.SetOnHand(dto.Quantity);
        await _uow.SaveAsync();
    }

    public async Task AdjustStock(Guid userId, StockAdjustmentDto stockAdjustmentDTO)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (stockAdjustmentDTO.ActualQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        var userWarehouseIds = user.UserWarehouses.Select(uwh => uwh.WarehouseId);
        if (!userWarehouseIds.Contains(stockAdjustmentDTO.WarehouseId))
            throw new ForbiddenException("You do not have access to this warehouse.");

        var productVariantId = stockAdjustmentDTO.ProductVariantId != Guid.Empty
            ? stockAdjustmentDTO.ProductVariantId
            : stockAdjustmentDTO.ProductId;

        var stockLevel = await _repo.GetAsync(
            productVariantId, stockAdjustmentDTO.WarehouseId);

        var diff = stockAdjustmentDTO.ActualQuantity - stockLevel.QuantityOnHand;
        if (diff == 0)
            return;

        stockLevel.SetOnHand(stockAdjustmentDTO.ActualQuantity);

        var stockMovement = new StockMovement
        {
            StockLevelId = stockLevel.Id,
            ProductVariantId = stockLevel.ProductVariantId,
            WarehouseId = stockLevel.WarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Adjustment,
            QuantityDelta = diff,
            ResultingOnHand = stockLevel.QuantityOnHand,
            ReferenceType = "StockAdjustment",
            ReferenceId = stockLevel.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Adjusted by {diff}. Reason: {stockAdjustmentDTO.Reason}"
        };

        await _stockMovementRepo.AddAsync(stockMovement);
        await _uow.SaveAsync();
    }

    public async Task<IEnumerable<LowStockDto>> lowStockDto(
        Guid warehouseId, Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var hasAccess = user.UserWarehouses
            .Any(uwh => uwh.WarehouseId == warehouseId);

        if (!hasAccess)
            throw new ForbiddenException(
                "The user doesn't have access to this warehouse.");

        return await _repo.GetLowStockAsync(warehouseId);
    }

    public async Task Transfer(Guid userId, TransferDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        if (dto.FromWarehouseId == dto.ToWarehouseId)
            throw new ArgumentException(
                "Source and destination warehouses must be different.");

        var warehouseIds = user.UserWarehouses
            .Select(uwh => uwh.WarehouseId)
            .ToHashSet();

        if (!warehouseIds.Contains(dto.FromWarehouseId) ||
            !warehouseIds.Contains(dto.ToWarehouseId))
        {
            throw new ForbiddenException(
                "You don't have access to both warehouses.");
        }

        var productVariantId = dto.ProductVariantId != Guid.Empty
            ? dto.ProductVariantId
            : dto.ProductId;

        var source = await _repo.GetAsync(productVariantId, dto.FromWarehouseId);

        if (source.AvailableQuantity < dto.Quantity)
            throw new ConflictException("Insufficient stock for this transfer.");

        var destination = await _repo.GetOrCreateAsync(
            productVariantId, dto.ToWarehouseId);

        source.DecreaseOnHand(dto.Quantity);
        destination.IncreaseOnHand(dto.Quantity);

        await _stockMovementRepo.AddAsync(new StockMovement
        {
            StockLevelId = source.Id,
            ProductVariantId = source.ProductVariantId,
            WarehouseId = dto.FromWarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Transfer,
            QuantityDelta = -dto.Quantity,
            ResultingOnHand = source.QuantityOnHand,
            ReferenceType = "StockTransfer",
            ReferenceId = source.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Transferred to warehouse {dto.ToWarehouseId}"
        });

        await _stockMovementRepo.AddAsync(new StockMovement
        {
            StockLevelId = destination.Id,
            ProductVariantId = destination.ProductVariantId,
            WarehouseId = dto.ToWarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Transfer,
            QuantityDelta = dto.Quantity,
            ResultingOnHand = destination.QuantityOnHand,
            ReferenceType = "StockTransfer",
            ReferenceId = destination.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Transferred from warehouse {dto.FromWarehouseId}"
        });

        await _uow.SaveAsync();
    }
}
