using InventoryManagementSystem.Domain.Exceptions;

namespace InventoryManagementSystem.Domain.Entities;

public class StockLevel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int QuantityOnHand { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public byte[] RowVersion { get; set; } = null!;

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
    public ICollection<StockReservation> Reservations { get; set; } = new List<StockReservation>();

    public void SetOnHand(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        if (quantity < ReservedQuantity)
            throw new ConflictException(
                "Quantity on hand cannot be less than the reserved quantity.");

        QuantityOnHand = quantity;
        Recalculate();
    }

    public void IncreaseOnHand(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        QuantityOnHand += quantity;
        Recalculate();
    }

    public void DecreaseOnHand(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new ConflictException("Insufficient available stock.");

        QuantityOnHand -= quantity;
        Recalculate();
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new ConflictException("Insufficient available stock.");

        ReservedQuantity += quantity;
        Recalculate();
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        ReservedQuantity = Math.Max(0, ReservedQuantity - quantity);
        Recalculate();
    }

    private void Recalculate()
    {
        AvailableQuantity = QuantityOnHand - ReservedQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
