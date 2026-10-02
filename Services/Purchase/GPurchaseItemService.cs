using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GPurchaseService
{
    private readonly BaseRepository<Purchase> _repo;

    public GPurchaseService(
        BaseRepository<Purchase> repo)
    {
        _repo = repo;
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

    public async Task<Purchase> Create(Purchase purchase)
    {
        var recod = await _repo.Add(purchase);
        if (recod is null)
            throw new Exception("sorry Purchase dont added try again");
        return purchase;

    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
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
    }
}