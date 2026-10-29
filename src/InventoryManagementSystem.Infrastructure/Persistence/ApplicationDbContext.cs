using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserWarehouse> UserWarehouses { get; set; } 
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; }

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

            modelBuilder.Entity<ProductVariant>(e =>
            {
                e.HasIndex(e => e.Sku).IsUnique();
                e.Property(e => e.Sku).HasMaxLength(64).IsRequired();

                e.HasOne(v => v.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(v => v.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductAttribute>(e =>
            {
                e.HasIndex(e => e.Code).IsUnique();
            });

            modelBuilder.Entity<ProductAttributeValue>(e =>
            {
                e.HasIndex(e => new { e.ProductAttributeId, e.Code }).IsUnique();
                e.HasOne(v => v.ProductAttribute)
                    .WithMany(a => a.Values)
                    .HasForeignKey(v => v.ProductAttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductVariantAttributeValue>(pv =>
            {
                pv.HasKey(x => new { x.ProductVariantId, x.ProductAttributeValueId });

                pv.HasOne(x => x.ProductVariant)
                    .WithMany(v => v.ProductVariantAttributeValues)
                    .HasForeignKey(x => x.ProductVariantId)
                    .OnDelete(DeleteBehavior.Cascade);

                pv.HasOne(x => x.ProductAttributeValue)
                    .WithMany(av => av.ProductVariantAttributeValues)
                    .HasForeignKey(x => x.ProductAttributeValueId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Stock>(s =>
            {
                s.HasIndex(s => new { s.ProductVariantId, s.WarehouseId }).IsUnique();

                s.HasOne(s => s.ProductVariant)
                    .WithMany(v => v.Stocks)
                    .HasForeignKey(s => s.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
    }
}
}