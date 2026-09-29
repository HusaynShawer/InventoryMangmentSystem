using InventoryMangmentSystem.Models;
namespace InventoryMangmentSystem.Repositories;

public interface ISaleItemRepository
{
    Task<IEnumerable<SaleItem>> GetAllAsync();
    Task<SaleItem?> GetByIdAsync(int id);
    //Task<Sale?> GetSaleWithItemsAsync(int id);
    Task<SaleItem> CreateAsync(SaleItem saleItem);
    Task UpdateAsync(SaleItem saleItem);
    Task Delete(int id);
}