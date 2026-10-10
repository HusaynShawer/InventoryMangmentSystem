using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Stock
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public int Quantity { get; set; }
    }
}