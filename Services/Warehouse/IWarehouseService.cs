using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface IWarehouseService
{
    Task<Warehouse> AddAsync(Warehouse warehouse);

    Task<IEnumerable<Warehouse>> GetAllAsync();

    Task<Warehouse> GetByIdAsync(int id);

    Task UpdateAsync(Warehouse warehouse);

    Task DeleteAsync(int id);
}