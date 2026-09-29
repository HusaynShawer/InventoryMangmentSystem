using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(int id);

    Task<IEnumerable<Warehouse>> GetAllAsync();

    Task<Warehouse> CreateAsync(Warehouse warehouse);

    Task UpdateAsync(Warehouse warehouse);

    Task DeleteAsync(int id);

}