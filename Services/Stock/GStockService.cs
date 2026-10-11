using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.RenderTree;
namespace InventoryMangmentSystem.Services;

public class GStockService
{
    private readonly StockRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly BaseRepository<StockMovement> _stockMovementRepo;
    private readonly BaseRepository<Product> _prodcutRepo;
    private readonly BaseRepository<Warehouse> _warehosueRepo;

    private readonly UnitOfWork _uow;
    public GStockService(
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
        public async Task<IEnumerable<Stock>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Stock found");
        return recods;
    }
    public async Task<Stock> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Stock Not found");
        return record;
    }
    public async Task<Stock> GetByProductId(Guid productId, Guid warehouseId)
    {
        var recod = await _repo.GetByProductIdAsync(productId,warehouseId);
        if (recod is null)
            throw new Exception("not stock for this product");
        return recod;
    }
        public async Task Delete(Guid id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
        await _uow.SaveAsync();
    }
    public async Task Update(Stock stock)
    {
        var record = await GetByID(stock.Id);
        record.Quantity = stock.Quantity;  
        _repo.Update(record);
        await _uow.SaveAsync();       
    }
    public async Task AdjustStock(Guid userId,StockAdjustmentDTO stockAdjustmentDTO)
    {

        var user = await _userRepo.GetByIdAsync(userId);
        if (stockAdjustmentDTO.ActualQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative");

        var userWarehouseIds = user.UserWarehouses.Select(uwh => uwh.WarehouseId);
        if(!userWarehouseIds.Contains(stockAdjustmentDTO.WarehouseId))
            throw new Exception("user didnt have this warehosue please add a valid warehosueId");
        
        
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
            MovementType = stockAdjustmentDTO.reason,
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
            throw new UnauthorizedAccessException(
                "The user doesn't have access to this warehouse.");
            
        return await _repo.GetLowStockAsync(warehouseId);
    }

    public async Task Transfer(Guid userId, TransferDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);        
        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        if (dto.formWarehouseId == dto.toWarehouseId)
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

        if (!warehouseIds.Contains(dto.formWarehouseId) ||
            !warehouseIds.Contains(dto.toWarehouseId))
        {
            throw new UnauthorizedAccessException(
                "You don't have access to both warehouses.");
        }

        var source = await _repo.GetByProductIdAsync(
            dto.productId, dto.formWarehouseId);
      
        if (source.Quantity < dto.Quantity)
            throw new InvalidOperationException(
              "Insufficient stock for this transfer.");

        var destination = await _repo.GetByProductIdAsync(
            dto.productId, dto.toWarehouseId);

        source.Quantity -= dto.Quantity;
        _repo.Update(source);

    // Increase destination stock or create its record.
        if (destination is null)
        {
            destination = new Stock
            {
                ProductId = dto.productId,
                WarehouseId = dto.toWarehouseId,
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
            ProductId = dto.productId,
            WarehouseId = dto.formWarehouseId,
            UserId = user.Id,
            Quantity = -dto.Quantity,
            MovementType = "TransferOut",
            ReferenceId = source.Id,
            Date = DateTime.UtcNow,
            Note = $"Transferred to warehouse {dto.toWarehouseId}"
        });

        await _stockMovementRepo.Add(new StockMovement
        {
            ProductId = dto.productId,
            WarehouseId = dto.toWarehouseId,
            UserId = user.Id,
            Quantity = dto.Quantity,
            MovementType = "TransferIn",
            ReferenceId = source.Id,
            Date = DateTime.UtcNow,
            Note = $"Transferred from warehouse {dto.formWarehouseId}"
        });

        await _uow.SaveAsync();
    }
}