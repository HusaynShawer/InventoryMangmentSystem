using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Sale
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();


        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [ForeignKey("CreatedByUser")]
        public Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}