namespace InventoryManagementSystem.Application.DTOs;
public class CartResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new List<CartItemResponseDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CartItemResponseDto
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; }
}

public class AddToCartRequestDto
{
    public Guid variantId { get; set; }
    public int quantity { get; set; }
    public Guid warehouseId { get; set; }
}

public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}