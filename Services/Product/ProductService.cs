using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Data;
namespace InventoryManagementSystem.Services;

public class ProductService
{
    private readonly BaseRepository<Product> _repo;
    private readonly UnitOfWork _unitOfWork;

    public ProductService(
        BaseRepository<Product> repo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }


    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id)
    {
        var product = await GetEntityOrThrow(id);
        return MapToDto(product);
    }


    public async Task<ProductResponseDto> Create(Guid userId, ProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            CategoryId = dto.CategoryId,
            UnitPrice = dto.UnitPrice,
            ReorderLevel = dto.ReorderLevel
        };

        if (dto.Stock is not null)
        {
            var defaultVariant = new ProductVariant
            {
                ProductId = product.Id,
                Sku = product.Sku ?? $"{product.Name?.Replace(" ", "-") ?? "PRD"}-{Guid.NewGuid().ToString()[..8]}".ToUpper(),
                IsDefault = true,
                IsActive = true
            };
            product.Variants.Add(defaultVariant);
            defaultVariant.Stocks.Add(new Stock
            {
                WarehouseId = dto.Stock.WarehouseId,
                Quantity = dto.Stock.Quantity
            });
        }

        await _repo.Add(product);
        await _unitOfWork.SaveAsync();

        return MapToDto(product);
    }


    public async Task<ProductResponseDto> Update(Guid id, ProductDto dto)
    {
        var product = await GetEntityOrThrow(id);

        product.Name = dto.Name;
        product.Sku = dto.Sku;
        product.CategoryId = dto.CategoryId;
        product.UnitPrice = dto.UnitPrice;
        product.ReorderLevel = dto.ReorderLevel;

        await _unitOfWork.SaveAsync();
        return MapToDto(product);
    }


    public async Task Delete(Guid id)
    {
        var product = await GetEntityOrThrow(id);
        _repo.Delete(product);
        await _unitOfWork.SaveAsync();     
    }

    // ---------- HELPERS ----------

    private async Task<Product> GetEntityOrThrow(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException("Product not found.");
        return product;
    }

    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
        CategoryId = p.CategoryId,
        UnitPrice = p.UnitPrice,
        ReorderLevel = p.ReorderLevel,
        Stocks = p.Stocks.Select(s => new StockResponseDto
        {
            Id = s.Id,
            WarehouseId = s.WarehouseId,
            Quantity = s.Quantity
        }).ToList(),
        Variants = p.Variants.Select(v => new ProductVariantResponseDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            Sku = v.Sku,
            Barcode = v.Barcode,
            PriceOverride = v.PriceOverride,
            CostOverride = v.CostOverride,
            EffectivePrice = v.PriceOverride ?? p.UnitPrice,
            IsActive = v.IsActive,
            IsDefault = v.IsDefault,
            Attributes = new List<string>()
        }).ToList()
    };
}