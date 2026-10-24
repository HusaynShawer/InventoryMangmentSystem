using InventoryManagementSystem.Data;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using System.Security.Claims;
namespace InventoryManagementSystem.Services;

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

    public async Task<IEnumerable<UserWarehouse>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
    public async Task<UserWarehouse> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("User warehouse assignment not found.");
        return record;
    }

    public async Task<UserWarehouse> Create(UserWarehouse userWarehouse)
    {
        var recod = await _repo.Add(userWarehouse);
        if (recod is null)
            throw new InvalidOperationException(
                "User warehouse assignment could not be created.");
        await _unitOfWork.SaveAsync();
        return userWarehouse;

    }
    public async Task Delete(Guid id)
    {
        var record = await GetByIdAsync(id);
        _repo.Delete(record);
    }
    // public async Task Update(Warehouse warehouse)
    // {
    //     var record = await GetByIdAsync(warehouse.Id);  
    //     record.Location = warehouse.Location;
    //     record.Name = warehouse.Name;
    //     _repo.Update(record);
    // }
}