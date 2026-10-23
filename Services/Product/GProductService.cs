using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagementSystem.Services;

public class ProductService
{
    private readonly BaseRepository<Product> _repo;
    private readonly UnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public ProductService(
        BaseRepository<Product> repo,
        UnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _context = context;
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

        Stock? initialStock = null;
        if (dto.Stock is not null)
        {
            initialStock = new Stock
            {
                WarehouseId = dto.Stock.WarehouseId,
                Quantity = dto.Stock.Quantity
            };
            product.Stocks.Add(initialStock);
        }

        await _repo.Add(product);
        await _unitOfWork.SaveAsync();

        if (initialStock is not null && dto.Stock is not null)
        {
            var stockMovement = new StockMovement
            {
                ProductId = product.Id,
                WarehouseId = dto.Stock.WarehouseId,
                UserId = userId,
                Quantity = dto.Stock.Quantity,
                MovementType = "Adjustment",
                ReferenceId = initialStock.Id,
                Date = DateTime.UtcNow,
                Note = $"Initial stock for new product"
            };
            _context.Set<StockMovement>().Add(stockMovement);
            await _unitOfWork.SaveAsync();
        }

        // Reload to include stocks
        product = await GetEntityOrThrow(product.Id);
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
        }).ToList()
    };
}