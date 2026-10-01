using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GsaleService
{
    private readonly BaseRepository<Sale> _repo;

    public GsaleService(
        BaseRepository<Sale> repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Sale>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No sales found");
        return recods;
    }
    public async Task<Sale> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Sale Not found");
        return record;
    }

    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Sale sale)
    {
        var record = await GetByID(sale.Id);
        record.WarehouseId = sale.WarehouseId;         
        record.CreatedByUserId = sale.CreatedByUserId; 
        record.TotalAmount = sale.TotalAmount;
        _repo.Update(record);
    }
}