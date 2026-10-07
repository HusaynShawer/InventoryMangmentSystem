using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
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
    

    public async Task<IEnumerable<CategoryResponseDTO>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Category found");
        return recods.Select(MapToDto).ToList();
    }
    public async Task<CategoryResponseDTO> GetByID(int id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Category Not found");
        return MapToDto(record);
    }

    public async Task<CategoryResponseDTO> Create(CategoryDTO dTO)
    {
        var recod = await _repo.Add(new Category{Name = dTO.Name});
        if (recod is null)
            throw new Exception("Sorry Category Not Added try again");
        await _unitOfWork.SaveAsync();
        return MapToDto(recod);
    }
    public async Task Delete(int id)
    {
        var record = await GetEntityOrThrow(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(int id, CategoryDTO dto)
    {
        var record = await GetEntityOrThrow(id);
        record.Name = dto.Name;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
    private async Task<Category> GetEntityOrThrow(int id)
    {
        var category = await _repo.GetById(id);
        if (category is null)
            throw new KeyNotFoundException("category not found");
        return category;
    }
    public CategoryResponseDTO MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };
}