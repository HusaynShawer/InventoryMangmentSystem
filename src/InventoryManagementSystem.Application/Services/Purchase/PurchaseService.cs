using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Application.DTOs;
namespace InventoryManagementSystem.Application.Services;

public class PurchaseService
{
    private readonly IRepository<Purchase> _repo;
    private readonly IRepository<StockMovement> _stockMovementRepo;
    private readonly IStockLevelRepository _stockLevelRepo;
    private readonly IProductVariantRepository _variantRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseService(
        IRepository<Purchase> repo,
        IRepository<StockMovement> stockMovementRepo,
        IStockLevelRepository stockLevelRepo,
        IProductVariantRepository variantRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockMovementRepo = stockMovementRepo;
        _stockLevelRepo = stockLevelRepo;
        _variantRepo = variantRepo;
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

            var purchaseItems = new List<PurchaseItem>();
            foreach (var item in dto.Items)
            {
                var variant = await ResolveVariantAsync(item.ProductId, item.ProductVariantId);

                purchaseItems.Add(new PurchaseItem
                {
                    ProductId = variant.ProductId,
                    ProductVariantId = variant.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            purchase.PurchaseItems = purchaseItems;

            purchase.TotalAmount = purchase.PurchaseItems
                .Sum(i => i.Quantity * i.UnitPrice);

            await _repo.AddAsync(purchase);

            await _unitOfWork.SaveAsync();

            foreach (var item in purchase.PurchaseItems)
            {
                var stockLevel = await _stockLevelRepo.GetOrCreateAsync(
                    item.ProductVariantId!.Value,
                    purchase.WarehouseId);

                stockLevel.IncreaseOnHand(item.Quantity);

                var stockMovement = new StockMovement
                {
                    StockLevelId = stockLevel.Id,
                    ProductVariantId = stockLevel.ProductVariantId,
                    WarehouseId = purchase.WarehouseId,
                    UserId = purchase.CreatedByUserId,
                    MovementType = StockMovementType.PurchaseIn,
                    QuantityDelta = item.Quantity,
                    ResultingOnHand = stockLevel.QuantityOnHand,
                    ReferenceType = "Purchase",
                    ReferenceId = purchase.Id,
                    CreatedAt = DateTime.UtcNow,
                    Note = "Purchase operation"
                };

                await _stockMovementRepo.AddAsync(stockMovement);
            }

            await _unitOfWork.SaveAsync();
            return purchase;
        });
    }

    private async Task<ProductVariant> ResolveVariantAsync(Guid productId, Guid productVariantId)
    {
        if (productVariantId != Guid.Empty)
        {
            return await _variantRepo.GetByIdAsync(productVariantId)
                ?? throw new NotFoundException("Product variant not found.");
        }

        var variants = (await _variantRepo.GetByProductIdAsync(productId)).ToList();
        var selected = variants.FirstOrDefault(v => v.IsDefault) ?? variants.FirstOrDefault();
        if (selected is null)
            throw new NotFoundException("Product variant not found.");

        return await _variantRepo.GetByIdAsync(selected.Id)
            ?? throw new NotFoundException("Product variant not found.");
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