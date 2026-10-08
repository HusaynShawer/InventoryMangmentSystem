using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using Microsoft.AspNetCore.Http.HttpResults;
namespace InventoryMangmentSystem.Services;

public class GPurchaseService
{
    private readonly BaseRepository<Purchase> _repo;
    private readonly BaseRepository<StockMovement> _stockMovementRepo;
    private readonly StockRepository _stockRepo;
    private readonly UnitOfWork _unitOfWork;

    public GPurchaseService(
        BaseRepository<Purchase> repo,
        StockRepository stockRepo,
        BaseRepository<StockMovement> stockMovementRepo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockRepo = stockRepo;
        _stockMovementRepo = stockMovementRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Purchase>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Purchases found");
        return recods;
    }
    public async Task<Purchase> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("purchase Not found");
        return record;
    }

    public async Task<Purchase> Create(PurchaseCreateDto dto)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
                CreatedByUserId = dto.CreatedByUserId,
                Date = dto.Date ?? DateTime.UtcNow
            };

            purchase.PurchaseItems = dto.Items
                .Select(i => new PurchaseItems
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })
                .ToList();

            purchase.TotalAmount = purchase.PurchaseItems
                .Sum(i => i.Quantity * i.UnitPrice);

            await _repo.Add(purchase);

            await _unitOfWork.SaveAsync();

            foreach (var item in purchase.PurchaseItems)
            {
                var stock = await _stockRepo.GetByProductIdAsync(
                    item.ProductId,
                    purchase.WarehouseId
                );

                if (stock is null)
                    throw new Exception(
                        $"Stock not found for ProductId: {item.ProductId}"
                    );

                stock.Quantity += item.Quantity;

                _stockRepo.Update(stock);

                var stockMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = purchase.WarehouseId,
                    UserId = purchase.CreatedByUserId,
                    Quantity = item.Quantity,
                    MovementType = "Purchase",
                    ReferenceId = purchase.Id,
                    Date = DateTime.UtcNow,
                    Note = "Purchase operation"
                };

                await _stockMovementRepo.Add(stockMovement);
            }

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitTransactionAsync(transaction);

            return purchase;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(transaction);
            throw;
        }
    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        await _unitOfWork.SaveAsync();
        _repo.Delete(record);
    }
    public async Task Update(Purchase purchase)
    {
        var record = await GetByID(purchase.Id);
        record.SupplierId = purchase.SupplierId;
        record.WarehouseId = purchase.WarehouseId;         
        record.CreatedByUserId = purchase.CreatedByUserId; 
        record.Date = purchase.Date;
        record.TotalAmount = purchase.TotalAmount;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
}