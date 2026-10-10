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