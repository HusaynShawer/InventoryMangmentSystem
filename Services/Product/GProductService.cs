using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using InventoryMangmentSystem.Data;
namespace InventoryMangmentSystem.Services;

public class GProductService
{
    private readonly BaseRepository<Product> _repo;
    private readonly UnitOfWork _unitOfWork;

    public GProductService(
        BaseRepository<Product> repo,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }


    public async Task<IEnumerable<ProductResponseDTO>> GetAll()
    {
        var products = await _repo.GetAll();
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductResponseDTO> GetByID(int id)
    {
        var product = await GetEntityOrThrow(id);
        return MapToDto(product);
    }


    public async Task<ProductResponseDTO> Create(int userId, ProductDTO dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            CategoryId = dto.CategoryId,
            UnitPrice = dto.UnitPrice,
            ReorderLevel = dto.ReorderLevel
        };

        if (dto.stock is not null)
        {
            product.Stocks.Add(new Stock
            {
                WarehouseId = dto.stock.WarehouseId,
                Quantity = dto.stock.Quantity
            });
        }

        await _repo.Add(product);
        await _unitOfWork.SaveAsync();

        return MapToDto(product);
    }


    public async Task<ProductResponseDTO> Update(int id, ProductDTO dto)
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


    public async Task Delete(int id)
    {
        var product = await GetEntityOrThrow(id);
        _repo.Delete(product);
        await _unitOfWork.SaveAsync();     
    }

    // ---------- HELPERS ----------

    private async Task<Product> GetEntityOrThrow(int id)
    {
        var product = await _repo.GetById(id);
        if (product is null)
            throw new KeyNotFoundException("Product not found");
        return product;
    }

    private static ProductResponseDTO MapToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
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