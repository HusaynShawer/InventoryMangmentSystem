using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GWarehouseService
{
    private readonly BaseRepository<Warehouse> _repo;

    public GWarehouseService(
        BaseRepository<Warehouse> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Warehouse>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Warehouse found");
        return recods;
    }
    public async Task<Warehouse> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Warehouse Not found");
        return record;
    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Warehouse warehouse)
    {
        var record = await GetByID(warehouse.Id);  
        record.Location = warehouse.Location;
        record.Name = warehouse.Name;
        _repo.Update(record);
    }
}