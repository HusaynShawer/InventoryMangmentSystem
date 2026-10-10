namespace InventoryMangmentSystem.Schemas;
public class PurchaseCreateDto
{
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? Date { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public class PurchaseItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}