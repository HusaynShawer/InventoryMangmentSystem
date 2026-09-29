using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface ISaleItemService
{
    Task<SaleItem> AddAsync(SaleItem saleItem);

    Task<IEnumerable<SaleItem>> GetAllAsync();

    Task<SaleItem> GetByIdAsync(int id);

    Task UpdateAsync(SaleItem saleItem);

    Task DeleteAsync(int id);
}