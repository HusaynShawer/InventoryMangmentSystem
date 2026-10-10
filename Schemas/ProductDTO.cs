using System.Collections.ObjectModel;
using System.Data.Common;

namespace InventoryMangmentSystem.Schemas;

public class ProductDTO
{
    public string Sku {get; set;}
    public string Name {get; set;}
    public Guid CategoryId {get; set;}
    public decimal UnitPrice {get; set;}
    public int ReorderLevel {get; set;}
    public StockDTO? stock{get; set;}
}

public class StockDTO
{
    public Guid WarehouseId {get; set;}
    public required int Quantity {get; set;}

}

public class ProductResponseDTO
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int ReorderLevel { get; set; }
    public List<StockResponseDTO> Stocks { get; set; } = new();
}

public class StockResponseDTO
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class StockMovementDTO
{
    public Guid ProductId{get; set;}
    public Guid WarehouseId {get; set;}
    public Guid UserId {get; set;}
    public int Quantity{get; set;}
    public int MovementType {get; set;}
    public Guid ReferenceId{get;set;}
    public DateTime Date { get; set;}
    public string? Note {get; set;}
}

    public class SupplierDTo
    {
        public Guid Id { get; set; }

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

        public Guid? SupplierId { get; set; }


    }