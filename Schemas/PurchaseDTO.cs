namespace InventoryMangmentSystem.Schemas;
public class PurchaseCreateDto
{
    public int SupplierId { get; set; }
    public int WarehouseId { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? Date { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public class PurchaseItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}