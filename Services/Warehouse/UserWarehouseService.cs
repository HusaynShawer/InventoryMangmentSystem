using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using System.Security.Claims;
namespace InventoryMangmentSystem.Services;

public class UserWarehouseService
{
    private readonly BaseRepository<UserWarehouse> _repo;
    private readonly UnitOfWork _unitOfWork;
    public UserWarehouseService(
        BaseRepository<UserWarehouse> repo,UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserWarehouse>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No UserWarehouse found");
        return recods;
    }
    public async Task<UserWarehouse> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Warehouse Not found");
        return record;
    }

    public async Task<UserWarehouse> Create(UserWarehouse userWarehouse)
    {
        var recod = await _repo.Add(userWarehouse);
        if (recod is null)
            throw new Exception("sorry User Warehouse item dont added try again");
        await _unitOfWork.SaveAsync();
        return userWarehouse;

    }
    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    // public async Task Update(Warehouse warehouse)
    // {
    //     var record = await GetByID(warehouse.Id);  
    //     record.Location = warehouse.Location;
    //     record.Name = warehouse.Name;
    //     _repo.Update(record);
    // }
}