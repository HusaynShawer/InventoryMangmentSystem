using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryMangmentSystem.Models
{
    public class UserWarehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [MaxLength(20)]
        public string AccessLevel { get; set; } = "Staff";

        public bool IsDefault { get; set; } = false;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}