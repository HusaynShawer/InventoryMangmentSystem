using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class Product
    {
        [Key] 
        public int Id { get; set; }

        public string? Sku { get; set; }

        public string? Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public decimal UnitPrice { get; set; }
        
        public int ReorderLevel { get; set; }

        public Category? Category { get; set; } 
    }
}