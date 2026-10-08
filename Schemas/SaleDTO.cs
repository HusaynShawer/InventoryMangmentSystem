using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Schemas;

public class SaleDTO
{
    public int WarehouseId {get; set;}
    public int CreatedByUserID{get; set;}
    public DateTime ? Date {get; set;}
    public decimal TotalAmount {get; set;}
    public List<SaleItemDTO> Items { get; set; } = new();

}

public class SaleItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleResponseDTO
{
    public int Id {get;set;}
    public int WarehouseId {get; set;}
    public int CreatedByUserID{get; set;}
    public DateTime ? Date {get; set;}
    public decimal TotalAmount {get; set;}
    public List<SaleItemDTO> Items { get; set; } = new();

}