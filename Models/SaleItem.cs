using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class SaleItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}