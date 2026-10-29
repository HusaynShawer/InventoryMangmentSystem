using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
namespace InventoryManagementSystem.Application.Services;

public class SaleService
{
    private readonly IRepository<Sale> _repo;
        private readonly IRepository<StockMovement> _stockMovementRepo;

    private readonly IStockRepository _stockRepo;
    private readonly IUnitOfWork _unitOfWork;
    public SaleService(
        IRepository<Sale> repo,
        IStockRepository stockRepo,
        IRepository<StockMovement> stockMovement,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockRepo = stockRepo;
        _stockMovementRepo = stockMovement;
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
                var _stock = await _stockRepo.GetByProductIdAsync(item.ProductId,_sale.WarehouseId);
                
                if (_stock is null)
                    throw new NotFoundException(
                        $"Stock not found for ProductId: {item.ProductId}"
                    );
                
                if(item.Quantity > _stock.Quantity)
                    throw new ConflictException(
                        "Insufficient stock: the requested quantity cannot be covered.");

                _stock.Quantity -= item.Quantity;
                _stockRepo.Update(_stock);
               
                var stockMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = _sale.WarehouseId,
                    UserId = _sale.CreatedByUserId,
                    Quantity = item.Quantity,
                    MovementType = "Purchase",
                    ReferenceId = _sale.Id,
                    Date = DateTime.UtcNow,
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