using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Infrastructure.Persistence;

/// <summary>
/// Assigns every existing warehouse to any ADMIN who has no warehouse
/// assignment yet. Without this, an admin cannot create product variants
/// because the variant endpoint requires an explicit UserWarehouse row.
/// </summary>
public static class UserWarehouseSeeder
{
    public static async Task SeedAdminWarehousesAsync(
        ApplicationDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var warehouseIds = await db.Warehouses
            .Select(w => w.Id)
            .ToListAsync(cancellationToken);

        if (warehouseIds.Count == 0)
            return;

        var admins = await db.Users
            .Where(u => u.Role == AppRoles.Admin)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (admins.Count == 0)
            return;

        var assigned = await db.UserWarehouses
            .Select(uw => new { uw.UserId, uw.WarehouseId })
            .ToListAsync(cancellationToken);

        var assignedSet = assigned
            .Select(a => (a.UserId, a.WarehouseId))
            .ToHashSet();

        var additions = new List<UserWarehouse>();
        foreach (var adminId in admins)
        {
            var first = true;
            foreach (var warehouseId in warehouseIds)
            {
                if (assignedSet.Contains((adminId, warehouseId)))
                {
                    first = false;
                    continue;
                }

                additions.Add(new UserWarehouse
                {
                    UserId = adminId,
                    WarehouseId = warehouseId,
                    AccessLevel = "Admin",
                    IsDefault = first,
                });
                first = false;
            }
        }

        if (additions.Count == 0)
            return;

        db.UserWarehouses.AddRange(additions);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Seeded {Count} warehouse assignment(s) for administrators.",
            additions.Count);
    }
}
