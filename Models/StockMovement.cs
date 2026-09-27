using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace InventoryMangmentSystem.Models
{
    public class StockMovement
    {
        [Key]
        public int Id {get; set;}

        [ForeignKey("products")]
        public int ProductId {get; set;}

        [ForeignKey("warehouse")]
        public int Warehoused {get; set;}
        public int Quantity {get; set;}
        public string MovementType {get; set;}
        public int ReferenceId {get; set;}
        public DateTime Date {get; set;}
        public string Note {get; set;}
    }
}