using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface ISaleService
{
    Task<Sale> AddAsync(Sale sale);

    Task<IEnumerable<Sale>> GetAllAsync();

    Task<Sale> GetByIdAsync(int id);

    Task UpdateAsync(Sale sale);

    Task DeleteAsync(int id);
}