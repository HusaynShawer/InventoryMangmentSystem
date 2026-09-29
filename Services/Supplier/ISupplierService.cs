using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services;

public interface ISupplierService
{
    Task<Supplier> AddAsync(Supplier supplier);

    Task<IEnumerable<Supplier>> GetAllAsync();

    Task<Supplier> GetByIdAsync(int id);

    Task UpdateAsync(Supplier supplier);

    Task DeleteAsync(int id);
}