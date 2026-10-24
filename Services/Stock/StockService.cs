using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.RenderTree;
namespace InventoryManagementSystem.Services;

public class StockService
{
    private readonly StockRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly BaseRepository<StockMovement> _stockMovementRepo;
    private readonly BaseRepository<Product> _prodcutRepo;
    private readonly BaseRepository<Warehouse> _warehosueRepo;

    private readonly UnitOfWork _uow;
    public StockService(
        StockRepository repo,
        IUserRepository userRepo,
        BaseRepository<StockMovement> stockMovementRepo,
        BaseRepository<Product> prodcutRepo,
        BaseRepository<Warehouse> warehouseRepo,
        
        UnitOfWork uow)
    {
        _repo = repo;
        _userRepo = userRepo;
        _stockMovementRepo = stockMovementRepo;
        _prodcutRepo = prodcutRepo;
        _warehosueRepo = warehouseRepo;
        _uow = uow;
    }

    public async Task<Stock> Create(Stock stock)
    {
        var result =  await _repo.Add(stock);
        await _uow.SaveAsync();
        return result;
    }
        public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
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

        var stockMovement = new StockMovement
        {
            ProductId = stock.ProductId,
            WarehouseId = stock.WarehouseId,
            UserId = user.Id,
            Quantity = diff,
            MovementType = stockAdjustmentDTO.Reason,
            ReferenceId = stock.Id,
            Date = DateTime.UtcNow,
            Note = $"the diff after adjustmnet {diff}"
        };
        await _stockMovementRepo.Add(stockMovement);
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
                ProductId = dto.ProductId,
                WarehouseId = dto.ToWarehouseId,
                Quantity = dto.Quantity
            };

            await _repo.Add(destination);
        }
        else
        {
            destination.Quantity += dto.Quantity;
            _repo.Update(destination);
        }

        await _stockMovementRepo.Add(new StockMovement
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.FromWarehouseId,
            UserId = user.Id,
            Quantity = -dto.Quantity,
            MovementType = "TransferOut",
            ReferenceId = source.Id,
            Date = DateTime.UtcNow,
            Note = $"Transferred to warehouse {dto.ToWarehouseId}"
        });

        await _stockMovementRepo.Add(new StockMovement
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.ToWarehouseId,
            UserId = user.Id,
            Quantity = dto.Quantity,
            MovementType = "TransferIn",
            ReferenceId = source.Id,
            Date = DateTime.UtcNow,
            Note = $"Transferred from warehouse {dto.FromWarehouseId}"
        });

        await _uow.SaveAsync();
    }
}