using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class PurchaseItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Purchase")]
        public Guid PurchaseId { get; set; }           
        public Purchase? Purchase { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}