using System.ComponentModel.DataAnnotations;

namespace InventoryMangmentSystem.Models
{
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}