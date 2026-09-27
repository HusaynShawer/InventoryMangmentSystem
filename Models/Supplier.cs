using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace InventoryMangmentSystem.Models
{
    public class Supplier
    {
        [Key]
        public int Id {get; set;}

        public string Name {get; set;}
        public string ContactInfo {get; set;}
        

    }
}