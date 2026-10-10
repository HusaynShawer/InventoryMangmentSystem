using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GStockService
{
    private readonly StockRepository _repo;
    private readonly UnitOfWork _uow;
    public GStockService(
        StockRepository repo,UnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Stock> Create(Stock stock)
    {
        var result =  await _repo.Add(stock);
        await _uow.SaveAsync();
        return result;
    }
        public async Task<IEnumerable<Stock>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Stock found");
        return recods;
    }
    public async Task<Stock> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Stock Not found");
        return record;
    }
        public async Task Delete(Guid id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Stock stock)
    {
        var record = await GetByID(stock.Id);
        record.Quantity = stock.Quantity;  
        await _uow.SaveAsync();       
        _repo.Update(record);
    }

}