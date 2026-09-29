using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class ProductService : IProductService
{

    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> AddAsync(Product product)
    {
        var record = await _repository.CreateAsync(product);
        if (record is null)
            throw new Exception("this product cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("product not found to delete it");
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var records = await _repository.GetAllAsync();
        if (!records.Any())
            throw new Exception("No product found try to add one");
        return records; 
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var record = await _repository.GetByIdAsync(id);
        if (record is null)
            throw new Exception("Product not found");
        return record;   
     }

    public async Task UpdateAsync(Product product)
    {
        var record = await GetByIdAsync(product.Id);
        if (record is null)
            throw new Exception("product not found to update");
        await _repository.UpdateAsync(product);
    }
}