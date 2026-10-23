using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repositories;

public class BaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await GetAllAsync();
    }

    public async Task<T?> GetById(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task<T> Add(T entity)
    {
        return await AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void UpdateAsync(T entity)
    {
        Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void DeleteAsync(T entity)
    {
        Delete(entity);
    }
}
