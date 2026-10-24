using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using InventoryMangmentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryMangmentSystem.Services;

public class GProductService
{
    private readonly BaseRepository<Product> _repo;
    private readonly UnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public GProductService(
        BaseRepository<Product> repo,
        UnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetAll()
    {
        var products = await _context.Products
            .Include(p => p.Stocks)
            .ToListAsync();

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDTO> GetByID(Guid id)
    {
        var product = await GetEntityOrThrow(id);
        return MapToDto(product);
    }

    public async Task<ProductResponseDTO> Create(Guid userId, ProductDTO dto)
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
        if (dto.stock is not null)
        {
            initialStock = new Stock
            {
                WarehouseId = dto.stock.WarehouseId,
                Quantity = dto.stock.Quantity
            };
            product.Stocks.Add(initialStock);
        }

        await _repo.Add(product);
        await _unitOfWork.SaveAsync();

        if (initialStock is not null && dto.stock is not null)
        {
            var stockMovement = new StockMovement
            {
                ProductId = product.Id,
                WarehouseId = dto.stock.WarehouseId,
                UserId = userId,
                Quantity = dto.stock.Quantity,
                MovementType = "Adjustment",
                ReferenceId = initialStock.Id,
                Date = DateTime.UtcNow,
                Note = "Initial stock for new product"
            };
            _context.StockMovements.Add(stockMovement);
            await _unitOfWork.SaveAsync();
        }

        product = await GetEntityOrThrow(product.Id);
        return MapToDto(product);
    }

    public async Task<ProductResponseDTO> Update(Guid id, ProductDTO dto)
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
        var product = await _context.Products
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException("Product not found.");

        return product;
    }

    private static ProductResponseDTO MapToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku ?? string.Empty,
        Name = p.Name ?? string.Empty,
        CategoryId = p.CategoryId,
        UnitPrice = p.UnitPrice,
        ReorderLevel = p.ReorderLevel,
        Stocks = p.Stocks.Select(s => new StockResponseDTO
        {
            Id = s.Id,
            WarehouseId = s.WarehouseId,
            Quantity = s.Quantity
        }).ToList()
    };
}
