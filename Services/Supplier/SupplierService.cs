using InventoryManagementSystem.Data;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
namespace InventoryManagementSystem.Services;

public class SupplierService
{
    private readonly BaseRepository<Supplier> _repo;
    private readonly UnitOfWork _unitOfWork;
    public SupplierService(
        BaseRepository<Supplier> repo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<Supplier> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Supplier not found.");
        return record;
    }
    public async Task<Supplier> Create(Supplier supplier)
    {
        var recod = await _repo.Add(supplier);
        if (recod is null)
            throw new InvalidOperationException(
                "Supplier could not be created.");
        await _unitOfWork.SaveAsync();
        return supplier;

    }

    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(Supplier supplier)
    {
        var record = await GetByIdAsync(supplier.Id);  
        record.Name = supplier.Name;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
}