using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Admin | Supplier | User
        [Required, MaxLength(20)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        public ICollection<UserWarehouse> UserWarehouses { get; set; } = new List<UserWarehouse>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}