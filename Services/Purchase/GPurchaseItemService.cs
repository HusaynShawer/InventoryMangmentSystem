using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using Microsoft.AspNetCore.Http.HttpResults;
namespace InventoryMangmentSystem.Services;

public class GPurchaseService
{
    private readonly BaseRepository<Purchase> _repo;
    private readonly StockRepository _stockRepo;

    private readonly UnitOfWork _unitOfWork;
    public GPurchaseService(
        BaseRepository<Purchase> repo,
        StockRepository stockRepo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _stockRepo = stockRepo;
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

        var _purchase = new Purchase
        {
            SupplierId = dto.SupplierId,
            WarehouseId = dto.WarehouseId,
            CreatedByUserId = dto.CreatedByUserId,
            Date = dto.Date ?? DateTime.UtcNow
        };

        _purchase.PurchaseItems = dto.Items.Select(i => new PurchaseItems{
                    ProductId = i.ProductId,
                    Quantity  = i.Quantity,
                    UnitPrice = i.UnitPrice
        }).ToList();
        
        _purchase.TotalAmount = _purchase.PurchaseItems.Sum(i => i.Quantity * i.UnitPrice);

        foreach (var item in _purchase.PurchaseItems)
        {
            var _stock = await _stockRepo.GetByProductIdAsync(item.ProductId,_purchase.WarehouseId);
            if (_stock is null)
                throw new Exception("this product not found to make update in purchase try to add it first");
            _stock.Quantity += item.Quantity;
            _stockRepo.Update(_stock);
        }
        await _repo.Add(_purchase);
        await _unitOfWork.SaveAsync();
        return _purchase;

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
        await _unitOfWork.SaveAsync();
        _repo.Update(record);
    }
}