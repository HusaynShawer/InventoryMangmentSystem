using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;

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
        public DbSet<StockLevel> StockLevels { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<StockReservation> StockReservations { get; set; }
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


            modelBuilder.Entity<StockLevel>(s =>
            {
                s.ToTable("StockLevels");
                s.HasIndex(s => new { s.ProductVariantId, s.WarehouseId }).IsUnique();
                s.Property(s => s.RowVersion).IsRowVersion();

                s.HasOne(s => s.ProductVariant)
                    .WithMany(v => v.StockLevels)
                    .HasForeignKey(s => s.ProductVariantId)
                    .OnDelete(DeleteBehavior.Restrict);

                s.HasOne(s => s.Warehouse)
                    .WithMany(w => w.StockLevels)
                    .HasForeignKey(s => s.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockMovement>(e =>
            {
                e.ToTable("StockMovements");
                e.HasKey(m => m.Id);

                e.Property(m => m.MovementType)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                e.Property(m => m.ReferenceType).HasMaxLength(50);
                e.Property(m => m.IdempotencyKey).HasMaxLength(100);
                e.Property(m => m.Note).HasMaxLength(500);

                e.HasOne(m => m.StockLevel)
                    .WithMany(s => s.Movements)
                    .HasForeignKey(m => m.StockLevelId)
                    .OnDelete(DeleteBehavior.Restrict); 

                e.HasIndex(m => m.IdempotencyKey)
                    .IsUnique()
                    .HasFilter("[IdempotencyKey] IS NOT NULL");

                e.HasIndex(m => new { m.ReferenceType, m.ReferenceId });
                e.HasIndex(m => new { m.StockLevelId, m.CreatedAt });
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

            modelBuilder.Entity<Cart>(c =>
            {
                c.Property(c => c.Status).HasConversion<string>()
                .HasMaxLength(32).IsRequired();

                c.HasIndex(c => c.UserId).IsUnique().HasFilter($"[{nameof(Cart.Status)}] = '{CartStatus.Active}'");

            c.HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(ci =>
            {
                ci.HasIndex(ci => new { ci.CartId, ci.ProductVariantId }).IsUnique();
            });

            modelBuilder.Entity<Order>(o =>
            {
                o.HasKey(o => o.Id);

                o.Property(o => o.OrderNumber).HasMaxLength(32).IsRequired();
                o.HasIndex(o => o.OrderNumber).IsUnique();

                o.Property(o => o.Status)
                .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                o.Property(o => o.Currency).HasMaxLength(3).IsRequired();
                o.Property(o => o.ShippingAddressSnapshot).HasMaxLength(500).IsRequired();

                o.Property(o => o.Subtotal).HasPrecision(18, 2);
                o.Property(o => o.ShippingAmount).HasPrecision(18, 2);
                o.Property(o => o.DiscountAmount).HasPrecision(18, 2);
                o.Property(o => o.TaxAmount).HasPrecision(18, 2);
                o.Property(o => o.TotalAmount).HasPrecision(18, 2);

                o.HasIndex(o => o.IdempotencyKey).IsUnique();

                // Concurrency
                o.Property(o => o.RowVersion).IsRowVersion();

                o.HasMany(o => o.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                o.HasIndex(o => new { o.UserId, o.Status });
            });

            modelBuilder.Entity<OrderItem>(oi =>
            {
                oi.Property(oi => oi.Status)
                .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                oi.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
                oi.Property(oi => oi.TotalPrice).HasPrecision(18, 2);

                oi.HasIndex(oi => new { oi.OrderId, oi.ProductVariantId }).IsUnique();
            });
            modelBuilder.Entity<StockReservation>(r =>
            {
                 r.ToTable("StockReservations");
                r.HasKey(r => r.Id);

                r.Property(r => r.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                r.HasOne(r => r.StockLevel)
                    .WithMany(s => s.Reservations)
                    .HasForeignKey(r => r.StockLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
    
                r.HasIndex(r => new { r.Status, r.ExpiresAt });

                r.HasIndex(r => r.OrderItemId)
                    .IsUnique()
                    .HasFilter("[Status] = 'Active'");
            });
        }
    }
}