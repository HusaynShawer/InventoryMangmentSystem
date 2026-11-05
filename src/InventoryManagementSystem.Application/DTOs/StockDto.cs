using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Application.DTOs;

public class StockDto
{
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public required int Quantity { get; set; }
}

public class StockResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
}

public class StockMovementDto
{
    public Guid StockLevelId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int QuantityDelta { get; set; }
    public int ResultingOnHand { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? IdempotencyKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Note { get; set; }
}

public class StockAdjustmentDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int ActualQuantity { get; set; }
}

public class LowStockDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class TransferDto
{
    public Guid ProductId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public int Quantity { get; set; }
}
