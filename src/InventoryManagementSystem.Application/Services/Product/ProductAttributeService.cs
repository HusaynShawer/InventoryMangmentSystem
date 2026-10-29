using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Services;

public class ProductAttributeService
{
    private readonly IProductAttributeRepository _repo;
    private readonly IProductAttributeValueRepository _valueRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ProductAttributeService(
        IProductAttributeRepository repo,
        IProductAttributeValueRepository valueRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _valueRepo = valueRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductAttributeResponseDto>> GetAllAsync()
    {
        var attributes = await _repo.GetAllAsync();
        return attributes.Select(MapToDto);
    }

    public async Task<ProductAttributeResponseDto> GetByIdAsync(Guid id)
    {
        var attribute = await _repo.GetByIdAsync(id);
        if (attribute is null)
            throw new NotFoundException("Product attribute not found.");
        return MapToDto(attribute);
    }

    public async Task<ProductAttributeResponseDto> CreateAsync(ProductAttributeDto dto)
    {
        var existing = await _repo.GetByCodeAsync(dto.Code);
        if (existing is not null)
            throw new ConflictException("Product attribute code already exists.");

        var attribute = new ProductAttribute
        {
            Name = dto.Name,
            Code = dto.Code
        };

        await _repo.AddAsync(attribute);
        await _unitOfWork.SaveAsync();
        var created = await _repo.GetByIdAsync(attribute.Id);
        return MapToDto(created!);
    }

    public async Task<ProductAttributeResponseDto> UpdateAsync(Guid id, ProductAttributeDto dto)
    {
        var attribute = await _repo.GetByIdAsync(id);
        if (attribute is null)
            throw new NotFoundException("Product attribute not found.");

        attribute.Name = dto.Name;
        attribute.Code = dto.Code;

        _repo.Update(attribute);
        await _unitOfWork.SaveAsync();
        var updated = await _repo.GetByIdAsync(attribute.Id);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var attribute = await _repo.GetByIdAsync(id);
        if (attribute is null)
            throw new NotFoundException("Product attribute not found.");
        _repo.Delete(attribute);
        await _unitOfWork.SaveAsync();
    }

    public async Task<ProductAttributeValueResponseDto> AddValueAsync(Guid attributeId, ProductAttributeValueDto dto)
    {
        var attribute = await _repo.GetByIdAsync(attributeId);
        if (attribute is null)
            throw new NotFoundException("Product attribute not found.");

        if (attribute.Values.Any(v => v.Code == dto.Code))
            throw new ConflictException("Attribute value code already exists.");

        var value = new ProductAttributeValue
        {
            ProductAttributeId = attributeId,
            Value = dto.Value,
            Code = dto.Code
        };

        await _valueRepo.AddAsync(value);
        await _unitOfWork.SaveAsync();
        var created = await _valueRepo.GetByIdAsync(value.Id);
        return MapValueToDto(created!);
    }

    public async Task DeleteValueAsync(Guid attributeId, Guid valueId)
    {
        var value = await _valueRepo.GetByIdAsync(valueId);
        if (value is null || value.ProductAttributeId != attributeId)
            throw new NotFoundException("Product attribute value not found.");
        _valueRepo.Delete(value);
        await _unitOfWork.SaveAsync();
    }

    private static ProductAttributeResponseDto MapToDto(ProductAttribute a)
    {
        return new ProductAttributeResponseDto
        {
            Id = a.Id,
            Name = a.Name,
            Code = a.Code,
            Values = a.Values.Select(MapValueToDto).ToList()
        };
    }

    private static ProductAttributeValueResponseDto MapValueToDto(ProductAttributeValue v)
    {
        return new ProductAttributeValueResponseDto
        {
            Id = v.Id,
            ProductAttributeId = v.ProductAttributeId,
            Value = v.Value,
            Code = v.Code
        };
    }
}
