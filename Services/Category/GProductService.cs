using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Services;

public class GCategoryService
{
    private readonly BaseRepository<Category> _repo;
    private readonly UnitOfWork _unitOfWork;
    public GCategoryService(
        BaseRepository<Category> repo,UnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.SaveAsync();
        return category;

    }
    public async Task Delete(int id)
    {
        var record = await GetByID(id);
        await _unitOfWork.SaveAsync();
        _repo.Delete(record);
    }
    public async Task Update(Category category)
    {
        var record = await GetByID(category.Id);
        record.Name = category.Name;
        await _unitOfWork.SaveAsync();
        _repo.Update(record);
    }
}