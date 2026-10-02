using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GSupplierService
{
    private readonly BaseRepository<Supplier> _repo;
    private readonly UnitOfWork _unitOfWork;
    public GSupplierService(
        BaseRepository<Supplier> repo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Supplier>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Suppliers found");
        return recods;
    }
    public async Task<Supplier> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Supplier Not found");
        return record;
    }
    public async Task<Supplier> Create(Supplier supplier)
    {
        var recod = await _repo.Add(supplier);
        if (recod is null)
            throw new Exception("sorry Supplier item dont added try again");
        await _unitOfWork.SaveAsync();
        return supplier;

    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(Supplier supplier)
    {
        var record = await GetByID(supplier.Id);  
        record.Name = supplier.Name;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
}