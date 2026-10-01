using Microsoft.EntityFrameworkCore;
using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserWarehouse> UserWarehouses { get; set; }   // ✅ جديد
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItems> PurchaseItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.UserName).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();

                e.HasOne(u => u.Supplier)
                 .WithMany(s => s.Users)
                 .HasForeignKey(u => u.SupplierId)
                 .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<UserWarehouse>(e =>
            {
                e.HasIndex(uw => new { uw.UserId, uw.WarehouseId }).IsUnique();

                e.HasOne(uw => uw.User)
                 .WithMany(u => u.UserWarehouses)
                 .HasForeignKey(uw => uw.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(uw => uw.Warehouse)
                 .WithMany(w => w.UserWarehouses)
                 .HasForeignKey(uw => uw.WarehouseId)
                 .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Purchase>(e =>
            {
                e.HasOne(p => p.Supplier)
                 .WithMany(s => s.Purchases)
                 .HasForeignKey(p => p.SupplierId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.Warehouse)
                 .WithMany(w => w.Purchases)
                 .HasForeignKey(p => p.WarehouseId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.CreatedByUser)
                 .WithMany(u => u.Purchases)
                 .HasForeignKey(p => p.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<Sale>(e =>
            {
                e.HasOne(s => s.Warehouse)
                 .WithMany(w => w.Sales)
                 .HasForeignKey(s => s.WarehouseId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(s => s.CreatedByUser)
                 .WithMany(u => u.Sales)
                 .HasForeignKey(s => s.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<StockMovement>(e =>
            {
                e.HasOne(sm => sm.User)
                 .WithMany(u => u.StockMovements)
                 .HasForeignKey(sm => sm.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}