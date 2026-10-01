using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class GPurchaseItemsService
{
    private readonly BaseRepository<PurchaseItems> _purchaseItemsRepository;

    public GPurchaseItemsService(
        BaseRepository<PurchaseItems> purchaseItemsRepository)
    {
        _purchaseItemsRepository = purchaseItemsRepository;
    }

    public async Task<IEnumerable<PurchaseItems>> GetAll()
    {
        var _purchaseItems =  await _purchaseItemsRepository.GetAll();
        if (!_purchaseItems.Any())
            throw new Exception("No purchaseitems found");
        return _purchaseItems;
    }
    public async Task<PurchaseItems> GetByID(int id)
    {
        var _purchaseItem = await _purchaseItemsRepository.GetById(id);
        if (_purchaseItem is null)
            throw new Exception("purchase item Not found");
        return _purchaseItem;
    }

    public async Task Delete(int id)
    {
        var purchaseItem = await GetByID(id);
        _purchaseItemsRepository.Delete(purchaseItem);
    }
    public async Task Update(PurchaseItems purchaseItem)
    {
        var record = await GetByID(purchaseItem.Id);
        record.Quantity = purchaseItem.Quantity;
        record.UnitPrice = purchaseItem.UnitPrice;
        _purchaseItemsRepository.Update(record);
    }
}