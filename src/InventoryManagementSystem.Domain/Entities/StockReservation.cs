using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Domain.Entities;

public class StockReservation
{
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid StockLevelId { get; set; }
    public int Quantity { get; set; }
    public StockReservationStatus Status { get; set; } = StockReservationStatus.Active;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public StockLevel StockLevel { get; set; } = null!;
}