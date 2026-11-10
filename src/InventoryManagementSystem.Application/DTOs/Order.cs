using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Application.DTOs;


// fist dto
public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }

    // Financials
    public decimal Subtotal { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "EGP";

    public string ShippingAddressSnapshot { get; set; } = null!;
    
    public DateTime? ReservedAt { get; set; }
    public DateTime? ReservationExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public string ShippingAddressSnapshot { get; set; } = null!;
    public Guid IdempotencyKey { get; set; }
    public string Currency { get; set; } = "EGP";
    
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    
    public ICollection<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}



public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderItemStatus Status { get; set; }
}

public class CreateOrderItemDto
{
    public Guid OrderId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}