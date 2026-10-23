namespace InventoryManagementSystem.DTOs;

public class ProductDto
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int ReorderLevel { get; set; }
    public StockDto? Stock { get; set; }
}

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int ReorderLevel { get; set; }
    public List<StockResponseDto> Stocks { get; set; } = new();
}

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}

public class UserDto
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
