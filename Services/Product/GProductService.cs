using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GProductService
{
    private readonly BaseRepository<Product> _repo;

    public GProductService(
        BaseRepository<Product> repo)
    {
        _repo = repo;
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
    public async Task<Product> Create(Product product)
    {
        var recod = await _repo.Add(product);
        if (recod is null)
            throw new Exception("sorry Product dont added try again");
        return product;

    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Product product)
    {
        var record = await GetByID(product.Id);
        record.Name = product.Name;
        record.Sku = product.Sku;
        record.UnitPrice = product.UnitPrice;
        _repo.Update(record);
    }
}