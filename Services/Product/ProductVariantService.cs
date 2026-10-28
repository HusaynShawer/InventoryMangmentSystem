using InventoryManagementSystem.Data;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;

namespace InventoryManagementSystem.Services;

public class ProductVariantService
{
    private readonly ProductVariantRepository _repo;
    private readonly BaseRepository<Product> _productRepo;
    private readonly UnitOfWork _unitOfWork;

    public ProductVariantService(
        ProductVariantRepository repo,
        BaseRepository<Product> productRepo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductVariantResponseDto>> GetByProductId(Guid productId)
    {
        return await _repo.GetByProductIdAsync(productId);
    }

    public async Task<ProductVariantResponseDto> GetById(Guid id)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant is null)
            throw new NotFoundException("Product variant not found.");
        return MapToDto(variant);
    }

    public async Task<ProductVariantResponseDto> Create(Guid productId, CreateProductVariantDto dto)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product is null)
            throw new NotFoundException("Product not found.");

        if (await _repo.GetBySkuAsync(dto.Sku) is not null)
            throw new ConflictException("Variant SKU already exists.");

        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = dto.Sku,
            Barcode = dto.Barcode,
            PriceOverride = dto.PriceOverride,
            CostOverride = dto.CostOverride,
            IsActive = true,
            IsDefault = false
        };

        if (dto.AttributeValueIds != null && dto.AttributeValueIds.Any())
        {
            foreach (var attrValId in dto.AttributeValueIds)
            {
                variant.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue
                {
                    ProductAttributeValueId = attrValId
                });
            }
        }

        await _repo.AddAsync(variant);
        await _unitOfWork.SaveAsync();
        var created = await _repo.GetByIdAsync(variant.Id);
        return MapToDto(created!);
    }

    public async Task<ProductVariantResponseDto> Update(Guid id, CreateProductVariantDto dto)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant is null)
            throw new NotFoundException("Product variant not found.");

        var existingBySku = await _repo.GetBySkuAsync(dto.Sku);
        if (existingBySku is not null && existingBySku.Id != id)
            throw new ConflictException("Variant SKU already exists.");

        variant.Sku = dto.Sku;
        variant.Barcode = dto.Barcode;
        variant.PriceOverride = dto.PriceOverride;
        variant.CostOverride = dto.CostOverride;

        variant.ProductVariantAttributeValues.Clear();
        if (dto.AttributeValueIds != null && dto.AttributeValueIds.Any())
        {
            foreach (var attrValId in dto.AttributeValueIds)
            {
                variant.ProductVariantAttributeValues.Add(new ProductVariantAttributeValue
                {
                    ProductVariantId = variant.Id,
                    ProductAttributeValueId = attrValId
                });
            }
        }

        _repo.Update(variant);
        await _unitOfWork.SaveAsync();
        var updated = await _repo.GetByIdAsync(variant.Id);
        return MapToDto(updated!);
    }

    public async Task Delete(Guid id)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant is null)
            throw new NotFoundException("Product variant not found.");
        _repo.Delete(variant);
        await _unitOfWork.SaveAsync();
    }

    private static ProductVariantResponseDto MapToDto(ProductVariant v)
    {
        var effectivePrice = v.PriceOverride ?? v.Product?.UnitPrice ?? 0;
        var attributes = v.ProductVariantAttributeValues
            .Where(x => x.ProductAttributeValue != null)
            .Select(x => $"{x.ProductAttributeValue!.ProductAttribute?.Name ?? ""}:{x.ProductAttributeValue!.Value}")
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
