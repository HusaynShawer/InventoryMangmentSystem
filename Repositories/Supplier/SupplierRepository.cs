using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Repositories;

public class SupplierRepository : ISupplierRepository
{
    public Task<Supplier> CreateAsync(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Supplier>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Supplier?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Supplier supplier)
    {
        throw new NotImplementedException();
    }
}