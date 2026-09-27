using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace InventoryMangmentSystem.Models
{
    public class Sale
    {
        [Key]
        public int Id {get; set;}
        public DateTime Date {get; set;}
        public decimal TotalAmount {get; set;}
    }
}