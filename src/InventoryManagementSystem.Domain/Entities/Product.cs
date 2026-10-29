using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(64)]
        public string? Sku { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int ReorderLevel { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }

    public class ProductAttribute
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public ICollection<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();
    }

    public class ProductAttributeValue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductAttributeId { get; set; }
        public ProductAttribute? ProductAttribute { get; set; }

        [Required]
        [StringLength(100)]
        public string Value { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public ICollection<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
    }

    public class ProductVariant
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(64)]
        public string Sku { get; set; } = string.Empty;

        [StringLength(128)]
        public string? Barcode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceOverride { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostOverride { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

        public ICollection<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }

    public class ProductVariantAttributeValue
    {
        public Guid ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public Guid ProductAttributeValueId { get; set; }
        public ProductAttributeValue? ProductAttributeValue { get; set; }
    }
}