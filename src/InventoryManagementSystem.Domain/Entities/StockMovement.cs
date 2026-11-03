using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StockLevelId { get; set; }
    public Guid ProductVariantId { get; set; }  
    public Guid WarehouseId { get; set; }

    public StockMovementType MovementType { get; set; }

    public int QuantityDelta { get; set; }

    public int ResultingOnHand { get; set; }

    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }

    public string? IdempotencyKey { get; set; }

    public string? Note { get; set; }

    public Guid? UserId { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public StockLevel StockLevel { get; set; } = null!;
}