using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Repositories;

public class UserWarehouseRepository : IUserWarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public UserWarehouseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserWarehouse>> GetByUserIdAsync(int userId)
    {
        return await _context.UserWarehouses
            .Where(uw => uw.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserWarehouse>> GetByWarehouseIdAsync(int warehouseId)
    {
        return await _context.UserWarehouses
            .Where(uw => uw.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<UserWarehouse> AssignAsync(UserWarehouse userWarehouse)
    {
        await _context.UserWarehouses.AddAsync(userWarehouse);
        await _context.SaveChangesAsync();
        return userWarehouse;
    }

    public async Task RemoveAsync(int userId, int warehouseId)
    {
        var record = await _context.UserWarehouses
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WarehouseId == warehouseId);

        if (record is null)
            throw new Exception("Assignment not found");

        _context.UserWarehouses.Remove(record);
        await _context.SaveChangesAsync();
    }
}