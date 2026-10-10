using System.ComponentModel.DataAnnotations;

namespace InventoryMangmentSystem.Models
{
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public ICollection<UserWarehouse> UserWarehouses { get; set; } = new List<UserWarehouse>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}