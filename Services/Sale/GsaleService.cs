using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Services;

public class GsaleService
{
    private readonly BaseRepository<Sale> _repo;
        private readonly BaseRepository<StockMovement> _stockMovementRepo;

    private readonly StockRepository _stockRepo;
    private readonly UnitOfWork _unitOfWork;
    public GsaleService(
        BaseRepository<Sale> repo,
        StockRepository stockRepo,
        BaseRepository<StockMovement> stockMovement,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockRepo = stockRepo;
        _stockMovementRepo = stockMovement;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Sale>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No sales found");
        return recods;
    }
    public async Task<Sale> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Sale Not found");
        return record;
    }
    public async Task<Sale> Create(SaleDTO dto)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var _sale = new Sale
            {
                CreatedByUserId = dto.CreatedByUserID,
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
            
            await _repo.Add(_sale);
            await _unitOfWork.SaveAsync();

            foreach(var item in _sale.SaleItems)
            {
                var _stock = await _stockRepo.GetByProductIdAsync(item.ProductId,_sale.WarehouseId);
                
                if (_stock is null)
                    throw new Exception(
                        $"Stock not found for ProductId: {item.ProductId}"
                    );
                
                if(item.Quantity > _stock.Quantity)
                    throw new Exception("Cant Cover Quantity");

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
                await _stockMovementRepo.Add(stockMovement);
            }

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync(transaction);
            return _sale;

        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(transaction);
            throw;
        }
        
    }

    public async Task Delete(Guid id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Sale sale)
    {
        var record = await GetByID(sale.Id);
        record.WarehouseId = sale.WarehouseId;         
        record.CreatedByUserId = sale.CreatedByUserId; 
        record.TotalAmount = sale.TotalAmount;
        _repo.Update(record);
    }
}