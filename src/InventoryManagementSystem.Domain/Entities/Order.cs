using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderNumber { get; set; } = null!;   // ORD-2026-00042
    public Guid UserId { get; set; }
    public Guid WarehouseId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Financials
    public decimal Subtotal { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "EGP";

    public string ShippingAddressSnapshot { get; set; } = null!;

    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();

    public DateTime? ReservedAt { get; set; }
    public DateTime? ReservationExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Concurrency
    public byte[] RowVersion { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Active;

    public Order Order { get; set; } = null!;
}