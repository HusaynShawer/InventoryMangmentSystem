using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using System.Security.Authentication;
namespace InventoryManagementSystem.Application.Services;

public class PurchaseItemService
{
    private readonly IRepository<PurchaseItem> _repo;
    private readonly IUnitOfWork _unitOfWork;
    public PurchaseItemService(
        IRepository<PurchaseItem> repo, IUnitOfWork unitOfWork)
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
        var recod = await _repo.AddAsync(purchaseItems);
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