using InventoryManagementSystem.Domain.Entities;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetAllAsync();

    Task<OrderItem?> GetByIdAsync(Guid id);

    Task<OrderItem> AddAsync(OrderItem orderItem);

    void Update(OrderItem orderItem);

    void Delete(OrderItem orderItem);
}