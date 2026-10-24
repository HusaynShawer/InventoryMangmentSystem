using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Repositories;
using System.Security.Authentication;
namespace InventoryManagementSystem.Services;

public class PurchaseItemService
{
    private readonly BaseRepository<PurchaseItem> _repo;
    private readonly UnitOfWork _unitOfWork;
    public PurchaseItemService(
        BaseRepository<PurchaseItem> repo, UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PurchaseItem>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<PurchaseItem> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Purchase item not found.");
        return record;
    }

    public async Task<PurchaseItem> Create(PurchaseItem purchaseItems)
    {
        var recod = await _repo.Add(purchaseItems);
        if (recod is null)
            throw new InvalidOperationException(
                "Purchase item could not be created.");
        await _unitOfWork.SaveAsync();
        return purchaseItems;

    }

    public async Task Delete(Guid id)
    {
        var purchaseItem = await GetByIdAsync(id);
        await _unitOfWork.SaveAsync();
        _repo.Delete(purchaseItem);
    }
    public async Task Update(PurchaseItem purchaseItem)
    {
        var record = await GetByIdAsync(purchaseItem.Id);
        record.Quantity = purchaseItem.Quantity;
        record.UnitPrice = purchaseItem.UnitPrice;
        await _unitOfWork.SaveAsync();
        _repo.Update(record);
    }
}