using System.Collections.ObjectModel;
using System.Data.Common;

namespace InventoryMangmentSystem.Schemas;

public class ProductDTO
{
    public string Sku {get; set;}
    public string Name {get; set;}
    public int CategoryId {get; set;}
    public decimal UnitPrice {get; set;}
    public int ReorderLevel {get; set;}
    public StockDTO? stock{get; set;}
}

public class StockDTO
{
    public int WarehouseId {get; set;}
    public required int Quantity {get; set;}

}

public class StockMovementDTO
{
    public int ProductId{get; set;}
    public int WarehouseId {get; set;}
    public int UserId {get; set;}
    public int Quantity{get; set;}
    public int MovementType {get; set;}
    public int ReferenceId{get;set;}
    public DateTime Date { get; set;}
    public string? Note {get; set;}
}

    public class SupplierDTo
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;

    }
 public class UserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        // Admin | Supplier | User
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? SupplierId { get; set; }


    }