using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Purchase // Fixed spelling
    {
        [Key]
        public int Id {get; set;}

        // Fixed FK naming convention
        public int SupplierId {get; set;}
        
        public DateTime Date {get; set;}
        public decimal TotalAmount {get; set;}

        // NAVIGATION PROPERTIES
        [ForeignKey("SupplierId")]
        public Supplier Supplier {get; set;}
        
        public ICollection<PurchaseItems> PurchaseItems {get; set;}
    }
}