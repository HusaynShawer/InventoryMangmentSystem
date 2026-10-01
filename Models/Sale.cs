using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("CreatedByUser")]
        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}