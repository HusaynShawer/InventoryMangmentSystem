using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class StockMovement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }


        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public int Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public Guid ReferenceId { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
    }
}