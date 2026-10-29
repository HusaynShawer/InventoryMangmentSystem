namespace InventoryManagementSystem.Application.DTOs;

public class SaleDto
{
    public Guid WarehouseId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? Date { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}

public class SaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleResponseDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? Date { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}
