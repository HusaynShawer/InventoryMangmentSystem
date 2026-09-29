using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
namespace InventoryMangmentSystem.Services;

public class SaleService : ISaleService
{

    private readonly ISaleRepository _sale;
    public SaleService(ISaleRepository sale)
    {
        _sale = sale;
    }

    public async Task<Sale> AddAsync(Sale sale)
    {
        var record = await _sale.CreateAsync(sale);
        if (record is null)
            throw new Exception("this sale cant added right now try letter!");
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var record = await GetByIdAsync(id);
        if (record is null)
            throw new Exception("sale not found to delete it");
        await _sale.Delete(id);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        var records = await _sale.GetAllSalesAsync();
        if (!records.Any())
            throw new Exception("No sales found try to add one");
        return records; 
    }

    public async Task<Sale> GetByIdAsync(int id)
    {
        var record = await _sale.GetSaleAsync(id);
        if (record is null)
            throw new Exception("sale not found");
        return record; 
    }

    public async Task UpdateAsync(Sale sale)
    {
        var record = await GetByIdAsync(sale.Id);
        if (record is null)
            throw new Exception("sale not found to update");
        await _sale.UpdateAsync(sale);
    }
}