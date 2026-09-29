using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Repositories;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);

    Task<IEnumerable<Supplier>> GetAllAsync();

    Task<Supplier> CreateAsync(Supplier supplier);

    Task UpdateAsync(Supplier supplier);

    Task DeleteAsync(int id);
}