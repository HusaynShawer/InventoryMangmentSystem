using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class GPurchaseItemsService
{
    private readonly BaseRepository<PurchaseItems> _repo;
    private readonly UnitOfWork _unitOfWork;
    public GPurchaseItemsService(
        BaseRepository<PurchaseItems> repo, UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PurchaseItems>> GetAll()
    {
        var records =  await _repo.GetAll();
        if (!records.Any())
            throw new Exception("No purchaseitems found");
        return records;
    }
    public async Task<PurchaseItems> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("purchase item Not found");
        return record;
    }

    public async Task<PurchaseItems> Create(PurchaseItems purchaseItems)
    {
        var recod = await _repo.Add(purchaseItems);
        if (recod is null)
            throw new Exception("sorry purchaseItems dont added try again");
        await _unitOfWork.SaveAsync();
        return purchaseItems;

    }

    public async Task Delete(int id)
    {
        var purchaseItem = await GetByID(id);
        await _unitOfWork.SaveAsync();
        _repo.Delete(purchaseItem);
    }
    public async Task Update(PurchaseItems purchaseItem)
    {
        var record = await GetByID(purchaseItem.Id);
        record.Quantity = purchaseItem.Quantity;
        record.UnitPrice = purchaseItem.UnitPrice;
        await _unitOfWork.SaveAsync();
        _repo.Update(record);
    }
}