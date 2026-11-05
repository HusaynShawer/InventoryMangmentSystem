using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
namespace InventoryManagementSystem.Application.Services;

public class SaleService
{
    private readonly IRepository<Sale> _repo;
    private readonly IRepository<StockMovement> _stockMovementRepo;
    private readonly IStockLevelRepository _stockLevelRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(
        IRepository<Sale> repo,
        IRepository<StockMovement> stockMovement,
        IStockLevelRepository stockLevelRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockMovementRepo = stockMovement;
        _stockLevelRepo = stockLevelRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<Sale> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Sale not found.");
        return record;
    }
    public async Task<Sale> Create(SaleDto dto)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var _sale = new Sale
            {
                CreatedByUserId = dto.CreatedByUserId,
                WarehouseId = dto.WarehouseId,
                Date = dto.Date ?? DateTime.UtcNow,
            };

            _sale.SaleItems = dto.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice

            }).ToList();

            _sale.TotalAmount = _sale.SaleItems.Sum(i =>i.Quantity * i.UnitPrice);
            
            await _repo.AddAsync(_sale);
            await _unitOfWork.SaveAsync();

            foreach(var item in _sale.SaleItems)
            {
                var stockLevel = await _stockLevelRepo.GetAsync(
                    item.ProductId, _sale.WarehouseId);

                if (item.Quantity > stockLevel.AvailableQuantity)
                    throw new ConflictException(
                        "Insufficient stock: the requested quantity cannot be covered.");

                stockLevel.DecreaseOnHand(item.Quantity);

                var stockMovement = new StockMovement
                {
                    StockLevelId = stockLevel.Id,
                    ProductVariantId = stockLevel.ProductVariantId,
                    WarehouseId = _sale.WarehouseId,
                    UserId = _sale.CreatedByUserId,
                    MovementType = StockMovementType.SaleOut,
                    QuantityDelta = -item.Quantity,
                    ResultingOnHand = stockLevel.QuantityOnHand,
                    ReferenceType = "Sale",
                    ReferenceId = _sale.Id,
                    CreatedAt = DateTime.UtcNow,
                    Note = "Sale operation"
                };
                await _stockMovementRepo.AddAsync(stockMovement);
            }

            await _unitOfWork.SaveAsync();
            return _sale;

        });
        
    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
    }
    public async Task Update(Sale sale)
    {
        var record = await GetByIdAsync(sale.Id);
        record.WarehouseId = sale.WarehouseId;         
        record.CreatedByUserId = sale.CreatedByUserId; 
        record.TotalAmount = sale.TotalAmount;
        _repo.Update(record);
    }
}