using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IUserWarehouseRepository
{
    Task<IEnumerable<UserWarehouse>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<UserWarehouse>> GetByWarehouseIdAsync(Guid warehouseId);

    Task<UserWarehouse> AssignAsync(UserWarehouse userWarehouse);

    Task RemoveAsync(Guid userId, Guid warehouseId);
}
