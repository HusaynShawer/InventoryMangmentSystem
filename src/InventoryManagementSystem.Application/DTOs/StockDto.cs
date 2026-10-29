namespace InventoryManagementSystem.Application.DTOs;

public class StockDto
{
    public Guid WarehouseId { get; set; }
    public required int Quantity { get; set; }
}

public class StockResponseDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class StockMovementDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public DateTime Date { get; set; }
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
