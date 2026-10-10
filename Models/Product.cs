using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Sku { get; set; }
        public string? Name { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<PurchaseItems> PurchaseItems { get; set; } = new List<PurchaseItems>();
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}