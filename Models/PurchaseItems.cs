using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class PurchaseItems
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Purchase")]
        public int PurchaseId { get; set; }           
        public Purchase? Purchase { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}