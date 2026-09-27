using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Purchase 
    {
        [Key]
        public int Id {get; set;}

        public int SupplierId {get; set;}
        
        public DateTime Date {get; set;}
        public decimal TotalAmount {get; set;}

        [ForeignKey("SupplierId")]
        public Supplier Supplier {get; set;}
        
        public ICollection<PurchaseItems> PurchaseItems {get; set;}
    }
}
