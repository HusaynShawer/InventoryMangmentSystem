using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Persistence.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly ApplicationDbContext _context;

    public ProductVariantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductVariantResponseDto>> GetByProductIdAsync(Guid productId)
    {
        var variants = await _context.ProductVariants
            .Include(v => v.Product)
            .Include(v => v.ProductVariantAttributeValues)
                .ThenInclude(pvav => pvav.ProductAttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Where(v => v.ProductId == productId)
            .ToListAsync();

        return variants.Select(MapToDto);
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id)
    {
        return await _context.ProductVariants
            .Include(v => v.Product)
            .Include(v => v.ProductVariantAttributeValues)
                .ThenInclude(pvav => pvav.ProductAttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<ProductVariant?> GetBySkuAsync(string sku)
    {
        return await _context.ProductVariants.FirstOrDefaultAsync(v => v.Sku == sku);
    }

    public async Task<ProductVariant> AddAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
        return variant;
    }

    public void Update(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
    }

    public void Delete(ProductVariant variant)
    {
        _context.ProductVariants.Remove(variant);
    }

    private static ProductVariantResponseDto MapToDto(ProductVariant v)
    {
        var effectivePrice = v.PriceOverride ?? v.Product?.UnitPrice ?? 0;
        var attributes = v.ProductVariantAttributeValues
            .Where(x => x.ProductAttributeValue != null)
            .Select(x => $"{x.ProductAttributeValue!.ProductAttribute?.Name ?? string.Empty}:{x.ProductAttributeValue!.Value}")
            .Where(x => !x.StartsWith(":"))
            .ToList();

        return new ProductVariantResponseDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            Sku = v.Sku,
            Barcode = v.Barcode,
            PriceOverride = v.PriceOverride,
            CostOverride = v.CostOverride,
            EffectivePrice = effectivePrice,
            IsActive = v.IsActive,
            IsDefault = v.IsDefault,
            Attributes = attributes
        };
    }
}
