using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IProductAttributeRepository
{
    Task<IEnumerable<ProductAttribute>> GetAllAsync();

    Task<ProductAttribute?> GetByIdAsync(Guid id);

    Task<ProductAttribute?> GetByCodeAsync(string code);

    Task<ProductAttribute> AddAsync(ProductAttribute attribute);

    void Update(ProductAttribute attribute);

    void Delete(ProductAttribute attribute);
}

public interface IProductAttributeValueRepository
{
    Task<ProductAttributeValue?> GetByIdAsync(Guid id);

    Task<ProductAttributeValue> AddAsync(ProductAttributeValue value);

    void Update(ProductAttributeValue value);

    void Delete(ProductAttributeValue value);
}
