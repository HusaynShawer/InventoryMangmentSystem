using InventoryManagementSystem.Models;
namespace InventoryManagementSystem.Repositories;
public interface IUserWarehouseRepository
{
    Task<IEnumerable<UserWarehouse>> GetByUserIdAsync(Guid userId);
    Task<UserWarehouse> AssignAsync(UserWarehouse uw);
    Task RemoveAsync(Guid userId, Guid warehouseId);
}