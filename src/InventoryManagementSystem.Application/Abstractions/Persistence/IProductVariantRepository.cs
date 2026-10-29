using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IProductVariantRepository
{
    Task<IEnumerable<ProductVariantResponseDto>> GetByProductIdAsync(Guid productId);

    Task<ProductVariant?> GetByIdAsync(Guid id);

    Task<ProductVariant?> GetBySkuAsync(string sku);

    Task<ProductVariant> AddAsync(ProductVariant variant);

    void Update(ProductVariant variant);

    void Delete(ProductVariant variant);
}
