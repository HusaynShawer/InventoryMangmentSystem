using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace InventoryMangmentSystem.Models
{
    public class Stock
    {
        [Key]
        public int Id {get; set;}
        [ForeignKey("product")]
        public int productId {get; set;}
        [ForeignKey("warehouse")]
        public int WarehouseId {get; set;}
        
        public int Quantity {get; set;}
    }
}