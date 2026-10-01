using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GSupplierService
{
    private readonly BaseRepository<Supplier> _repo;

    public GSupplierService(
        BaseRepository<Supplier> repo)
    {
        _repo = repo;
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

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Supplier supplier)
    {
        var record = await GetByID(supplier.Id);  
        record.Name = supplier.Name;
        _repo.Update(record);
    }
}