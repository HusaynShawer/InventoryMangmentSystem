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
    private readonly BaseRepository<StockMovement> _stockMovementRepo;
    private readonly UnitOfWork _uow;
    public GStockService(
        StockRepository repo,
        BaseRepository<StockMovement> stockMovementRepo,
        UnitOfWork uow)
    {
        _repo = repo;
        _stockMovementRepo = stockMovementRepo;
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
    public async Task AdjustStock(User user,StockAdjustmentDTO stockAdjustmentDTO)
    {
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

}