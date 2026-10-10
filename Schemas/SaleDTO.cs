using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Schemas;

public class SaleDTO
{
    public Guid WarehouseId {get; set;}
    public Guid CreatedByUserID{get; set;}
    public DateTime ? Date {get; set;}
    public decimal TotalAmount {get; set;}
    public List<SaleItemDTO> Items { get; set; } = new();

}

public class SaleItemDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleResponseDTO
{
    public Guid Id {get;set;}
    public Guid WarehouseId {get; set;}
    public Guid CreatedByUserID{get; set;}
    public DateTime ? Date {get; set;}
    public decimal TotalAmount {get; set;}
    public List<SaleItemDTO> Items { get; set; } = new();

}