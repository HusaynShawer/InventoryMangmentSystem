using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class StockMovement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public int Quantity { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
    }
}