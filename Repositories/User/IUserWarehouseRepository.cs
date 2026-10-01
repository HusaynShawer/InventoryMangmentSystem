using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;
public interface IUserWarehouseRepository
{
    Task<IEnumerable<UserWarehouse>> GetByUserIdAsync(int userId);
    Task<UserWarehouse> AssignAsync(UserWarehouse uw);
    Task RemoveAsync(int userId, int warehouseId);
}