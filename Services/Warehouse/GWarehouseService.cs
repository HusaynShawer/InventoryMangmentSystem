using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using System.Security.Claims;
namespace InventoryMangmentSystem.Services;

public class GWarehouseService
{
    private readonly BaseRepository<Warehouse> _repo;
    private readonly WarehouseRepository _warehouseRepository;
    private readonly UnitOfWork _unitOfWork;
    public GWarehouseService(
        BaseRepository<Warehouse> repo,WarehouseRepository warehouseRepository,UnitOfWork unitOfWork)
    {
        _repo = repo;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<Warehouse> Create(Warehouse warehouse)
    {
        var recod = await _repo.Add(warehouse);
        if (recod is null)
            throw new Exception("sorry Warehouse item dont added try again");
        await _unitOfWork.SaveAsync();
        return warehouse;

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

    public async Task<IEnumerable<Product>> GetWarehouseProducts(int userId)
    {
        var records = await _warehouseRepository.GetWarehouseProduct(userId);
        return records;
    }
    public async Task<IEnumerable<Product>> GetWarehouseLowProducts(int userId)
    {
        var records = await _warehouseRepository.GetWarehouseLowProducts(userId);
        return records;
    }
}