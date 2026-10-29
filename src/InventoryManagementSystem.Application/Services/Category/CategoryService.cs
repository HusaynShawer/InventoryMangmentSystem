using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
namespace InventoryManagementSystem.Application.Services;

public class CategoryService
{
    private readonly IRepository<Category> _repo;
    private readonly IUnitOfWork _unitOfWork;
    public CategoryService(
        IRepository<Category> repo,IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }
    

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var recods = await _repo.GetAllAsync();
        return recods.Select(MapToDto).ToList();
    }
    public async Task<CategoryResponseDto> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Category not found.");
        return MapToDto(record);
    }

    public async Task<CategoryResponseDto> Create(CategoryDto dTO)
    {
        var recod = await _repo.AddAsync(new Category{Name = dTO.Name});
        if (recod is null)
            throw new InvalidOperationException(
                "Category could not be created.");
        await _unitOfWork.SaveAsync();
        return MapToDto(recod);
    }
    public async Task Delete(Guid id)
    {
        var record = await GetEntityOrThrow(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(Guid id, CategoryDto dto)
    {
        var record = await GetEntityOrThrow(id);
        record.Name = dto.Name;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }
    private async Task<Category> GetEntityOrThrow(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
            throw new NotFoundException("Category not found.");
        return category;
    }
    public CategoryResponseDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };
}