using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace InventoryMangmentSystem.Models
{
    public class SaleItem
    {
        [Key]
        public int Id {get; set;}
        
        [ForeignKey("sale")]
        public int SaleId {get; set;}

        [ForeignKey("products")]
        public int ProductId {get; set;}
        public int Quantity {get; set;}
        public decimal UnitPrice {get; set;}
    }
}