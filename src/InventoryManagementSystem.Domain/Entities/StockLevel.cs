namespace InventoryManagementSystem.Domain.Entities;

public class StockLevel
{
    public Guid Id { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid WarehouseId { get; set; }

    public int QuantityOnHand { get; set; }      
    public int ReservedQuantity { get; set; }    
    public int AvailableQuantity { get; set; }   

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public byte[] RowVersion { get; set; } = null!;

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
    public ICollection<StockReservation> Reservations { get; set; } = new List<StockReservation>();
}