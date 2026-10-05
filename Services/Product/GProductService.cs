using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using InventoryMangmentSystem.Schemas;
namespace InventoryMangmentSystem.Services;

public class GProductService
{
    private readonly BaseRepository<Product> _repo;
    private readonly UnitOfWork _unitOfWork;
    private readonly StockRepository _stockRepo;
    public GProductService(
        BaseRepository<Product> repo,
        StockRepository stockRepository,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _stockRepo = stockRepository;
        _unitOfWork = unitOfWork;

    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No producst found");
        return recods;
    }
    public async Task<Product> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("product Not found");
        return record;
    }
    public async Task<Product> Create(int userId, ProductDTO dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            CategoryId = dto.CategoryId,
            UnitPrice = dto.UnitPrice,
            ReorderLevel = dto.ReorderLevel
        };
        var recod = await _repo.Add(product);
        
        if (recod is null)
            throw new Exception("sorry Product dont added try again");
       
        if(dto.stock is not null)
        {
            var stock = new Stock
            {
                ProductId = product.Id,
                WarehouseId = dto.stock.WarehouseId,
                Quantity = dto.stock.Quantity
            };

            var _stock = await _stockRepo.Add(stock);

        }
        await _unitOfWork.SaveAsync();
        return product;
    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        await _unitOfWork.SaveAsync();
        _repo.Delete(record);
    }
    public async Task Update(Product product)
    {
        var record = await GetByID(product.Id);
        record.Name = product.Name;
        record.Sku = product.Sku;
        record.UnitPrice = product.UnitPrice;
        await _unitOfWork.SaveAsync();
        _repo.Update(record);
    }
}