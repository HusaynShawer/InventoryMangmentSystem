using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllSalesAsync();
    Task<Sale?> GetSaleAsync(int id);
    //Task<Sale?> GetSaleWithItemsAsync(int id);
    Task<Sale> CreateAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task Delete(int id);
}