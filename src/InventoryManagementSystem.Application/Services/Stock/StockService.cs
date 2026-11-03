using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Application.Services;

public class StockService
{
    private readonly IStockRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IRepository<StockMovement> _stockMovementRepo;
    private readonly IStockLevelRepository _stockLevelRepo;
    private readonly IRepository<Product> _prodcutRepo;
    private readonly IWarehouseRepository _warehosueRepo;

    private readonly IUnitOfWork _uow;
    public StockService(
        IStockRepository repo,
        IUserRepository userRepo,
        IRepository<StockMovement> stockMovementRepo,
        IStockLevelRepository stockLevelRepo,
        IRepository<Product> prodcutRepo,
        IWarehouseRepository warehouseRepo,
        
        IUnitOfWork uow)
    {
        _repo = repo;
        _userRepo = userRepo;
        _stockMovementRepo = stockMovementRepo;
        _stockLevelRepo = stockLevelRepo;
        _prodcutRepo = prodcutRepo;
        _warehosueRepo = warehouseRepo;
        _uow = uow;
    }

    public async Task<Stock> Create(Stock stock)
    {
        var result =  await _repo.AddAsync(stock);
        await _stockLevelRepo.GetOrCreateForStockAsync(stock);
        await _uow.SaveAsync();
        return result;
    }
    public async Task<IEnumerable<StockResponseDto>> GetAllAsync(Guid userId)
    {
        var UserWarehousesIds = await _warehosueRepo.GetByUserId(userId);
        return await _repo.GetAllAsync(UserWarehousesIds.ToList());
    }
    public async Task<Stock> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Stock not found.");
        return record;
    }
    public async Task<Stock> GetByProductId(Guid productId, Guid warehouseId)
    {
        var recod = await _repo.GetByProductIdAsync(productId,warehouseId);
        if (recod is null)
            throw new NotFoundException(
                "No stock record found for this product in the given warehouse.");
        return recod;
    }
        public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
        await _uow.SaveAsync();
    }
    public async Task Update(Stock stock)
    {
        var record = await GetByIdAsync(stock.Id);
        record.Quantity = stock.Quantity;  
        _repo.Update(record);
        await _stockLevelRepo.GetOrCreateForStockAsync(record);
        await _uow.SaveAsync();       
    }
    public async Task AdjustStock(Guid userId,StockAdjustmentDto stockAdjustmentDTO)
    {

        var user = await _userRepo.GetByIdAsync(userId);
        if (stockAdjustmentDTO.ActualQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative");

        var userWarehouseIds = user.UserWarehouses.Select(uwh => uwh.WarehouseId);
        if(!userWarehouseIds.Contains(stockAdjustmentDTO.WarehouseId))
            throw new ForbiddenException(
                "You do not have access to this warehouse.");
        
        
        var stock = await GetByProductId(stockAdjustmentDTO.ProductId, stockAdjustmentDTO.WarehouseId);
        int diff = stockAdjustmentDTO.ActualQuantity - stock.Quantity;
        if (diff == 0)
            return;
        stock.Quantity = stockAdjustmentDTO.ActualQuantity;
        _repo.Update(stock);

        var stockLevel = await _stockLevelRepo.GetOrCreateForStockAsync(stock);

        var stockMovement = new StockMovement
        {
            StockLevelId = stockLevel.Id,
            ProductVariantId = stock.ProductVariantId,
            WarehouseId = stock.WarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Adjustment,
            QuantityDelta = diff,
            ResultingOnHand = stock.Quantity,
            ReferenceType = "StockAdjustment",
            ReferenceId = stock.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Adjusted by {diff}. Reason: {stockAdjustmentDTO.Reason}"
        };
        await _stockMovementRepo.AddAsync(stockMovement);
        await _uow.SaveAsync();
    }

    public async Task<IEnumerable<LowStockDto>> lowStockDto(Guid warehouseId,Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        var hasAccess = user.UserWarehouses
            .Any(uwh => uwh.WarehouseId == warehouseId);
        
        if (!hasAccess)
            throw new ForbiddenException(
                "The user doesn't have access to this warehouse.");
            
        return await _repo.GetLowStockAsync(warehouseId);
    }

    public async Task Transfer(Guid userId, TransferDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);        
        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        if (dto.FromWarehouseId == dto.ToWarehouseId)
            throw new ArgumentException(
                "Source and destination warehouses must be different.");

        var warehouseIds = user.UserWarehouses
            .Select(uwh => uwh.WarehouseId)
            .ToHashSet();

        foreach (var wu in warehouseIds)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" كدا المفروض يشتغل{wu}");
        }

        if (!warehouseIds.Contains(dto.FromWarehouseId) ||
            !warehouseIds.Contains(dto.ToWarehouseId))
        {
            throw new ForbiddenException(
                "You don't have access to both warehouses.");
        }

        var source = await _repo.GetByProductIdAsync(
            dto.ProductId, dto.FromWarehouseId);
      
        if (source == null)
            throw new NotFoundException("No stock record found for this product in the given warehouse.");

        if (source.Quantity < dto.Quantity)
            throw new ConflictException(
              "Insufficient stock for this transfer.");

        var destination = await _repo.GetByProductIdAsync(
            dto.ProductId, dto.ToWarehouseId);

        source.Quantity -= dto.Quantity;
        _repo.Update(source);

    // Increase destination stock or create its record.
        if (destination is null)
        {
            destination = new Stock
            {
                ProductVariantId = dto.ProductId,
                WarehouseId = dto.ToWarehouseId,
                Quantity = dto.Quantity
            };

            await _repo.AddAsync(destination);
        }
        else
        {
            destination.Quantity += dto.Quantity;
            _repo.Update(destination);
        }

        var sourceLevel = await _stockLevelRepo.GetOrCreateForStockAsync(source);
        var destinationLevel = await _stockLevelRepo.GetOrCreateForStockAsync(destination);

        await _stockMovementRepo.AddAsync(new StockMovement
        {
            StockLevelId = sourceLevel.Id,
            ProductVariantId = source.ProductVariantId,
            WarehouseId = dto.FromWarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Transfer,
            QuantityDelta = -dto.Quantity,
            ResultingOnHand = source.Quantity,
            ReferenceType = "StockTransfer",
            ReferenceId = source.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Transferred to warehouse {dto.ToWarehouseId}"
        });

        await _stockMovementRepo.AddAsync(new StockMovement
        {
            StockLevelId = destinationLevel.Id,
            ProductVariantId = destination.ProductVariantId,
            WarehouseId = dto.ToWarehouseId,
            UserId = user.Id,
            MovementType = StockMovementType.Transfer,
            QuantityDelta = dto.Quantity,
            ResultingOnHand = destination.Quantity,
            ReferenceType = "StockTransfer",
            ReferenceId = destination.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Transferred from warehouse {dto.FromWarehouseId}"
        });
        await _uow.SaveAsync();
    }
}