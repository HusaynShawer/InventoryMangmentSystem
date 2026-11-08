using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Application.Services;

public class ProductVariantService
{
    private readonly IProductVariantRepository _repo;
    private readonly IRepository<Product> _productRepo;
    private readonly IWarehouseRepository _warehouseRepo;
    private readonly IRepository<StockMovement> _stockMovement;
    private readonly IStockLevelRepository _stockLevelRepo;

    private readonly IUnitOfWork _unitOfWork;

    public ProductVariantService(
        IProductVariantRepository repo,
        IRepository<Product> productRepo,
        IWarehouseRepository warehouseRepo,
        IRepository<StockMovement> stockMovement,
        IStockLevelRepository stockLevelRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _productRepo = productRepo;
        _warehouseRepo = warehouseRepo;
        _stockMovement = stockMovement;
        _stockLevelRepo = stockLevelRepo;
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

    public async Task<ProductVariantResponseDto> Create(Guid productId, Guid userId, CreateProductVariantDto dto)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product is null)
            throw new NotFoundException("Product not found.");

        if (dto.WarehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse is required.");

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

        var userWarehouseIds = await _warehouseRepo.GetByUserId(userId);
        if (!userWarehouseIds.Contains(dto.WarehouseId))
            throw new ForbiddenException(
                "You do not have access to this warehouse.");

        await _repo.AddAsync(variant);
        await _unitOfWork.SaveAsync();

        var stockLevel = await _stockLevelRepo.GetOrCreateAsync(variant.Id, dto.WarehouseId);

        var movement = new StockMovement
        {
            StockLevelId = stockLevel.Id,
            ProductVariantId = variant.Id,
            WarehouseId = dto.WarehouseId,
            UserId = userId,
            MovementType = StockMovementType.Adjustment,
            QuantityDelta = 0,
            ResultingOnHand = stockLevel.QuantityOnHand,
            ReferenceType = "ProductVariant",
            ReferenceId = variant.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Variant {variant.Sku} created"
        };

        await _stockMovement.AddAsync(movement);
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

    public async Task Delete(Guid id, Guid userId, Guid warehouseId)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant is null)
            throw new NotFoundException("Product variant not found.");

        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse is required.");

        var userWarehouseIds = await _warehouseRepo.GetByUserId(userId);
        if (!userWarehouseIds.Contains(warehouseId))
            throw new ForbiddenException(
                "You do not have access to this warehouse.");

        var stockLevel = await _stockLevelRepo.GetOrCreateAsync(variant.Id, warehouseId);

        var movement = new StockMovement
        {
            StockLevelId = stockLevel.Id,
            ProductVariantId = variant.Id,
            WarehouseId = warehouseId,
            UserId = userId,
            MovementType = StockMovementType.Adjustment,
            QuantityDelta = 0,
            ResultingOnHand = stockLevel.QuantityOnHand,
            ReferenceType = "ProductVariant",
            ReferenceId = variant.Id,
            CreatedAt = DateTime.UtcNow,
            Note = $"Variant {variant.Sku} deleted"
        };

        await _stockMovement.AddAsync(movement);

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
