using System.ComponentModel.DataAnnotations;

namespace InventoryMangmentSystem.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}