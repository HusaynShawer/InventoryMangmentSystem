using InventoryManagementSystem.Data;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using InventoryManagementSystem.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
namespace InventoryManagementSystem.Services;

public class PurchaseService
{
    private readonly BaseRepository<Purchase> _repo;
    private readonly BaseRepository<StockMovement> _stockMovementRepo;
    private readonly StockRepository _stockRepo;
    private readonly UnitOfWork _unitOfWork;

    public PurchaseService(
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

    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<Purchase> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Purchase not found.");
        return record;
    }

    public async Task<Purchase> Create(PurchaseCreateDto dto)
    {
        return await _unitOfWork.ExecuteInTransactionAsync( async () =>
        {
            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
                CreatedByUserId = dto.CreatedByUserId,
                Date = dto.Date ?? DateTime.UtcNow
            };

            purchase.PurchaseItems = dto.Items
                .Select(i => new PurchaseItem
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
                    throw new NotFoundException(
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
            return purchase;
        });
    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(Purchase purchase)
    {
        var record = await GetByIdAsync(purchase.Id);
        record.SupplierId = purchase.SupplierId;
        record.WarehouseId = purchase.WarehouseId;         
        record.CreatedByUserId = purchase.CreatedByUserId; 
        record.Date = purchase.Date;
        record.TotalAmount = purchase.TotalAmount;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
}