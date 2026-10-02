using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GCategoryService
{
    private readonly BaseRepository<Category> _repo;
    private readonly UnitOfWork _uow;
    public GCategoryService(
        BaseRepository<Category> repo,UnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }
    

    public async Task<IEnumerable<Category>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Category found");
        return recods;
    }
    public async Task<Category> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Category Not found");
        return record;
    }

    public async Task<Category> Create(Category category)
    {
        var recod = await _repo.Add(category);
        if (recod is null)
            throw new Exception("sorry categord dont added try again");
        return category;

    }
    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        _repo.Delete(record);
    }
    public async Task Update(Category category)
    {
        var record = await GetByID(category.Id);
        record.Name = category.Name;
        _repo.Update(record);
    }
}