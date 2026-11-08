using InventoryManagementSystem.Domain.Entities;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();

    Task<Order?> GetByIdAsync(Guid id);

    Task<Order> AddAsync(Order order);

    void Update(Order order);

    void Delete(Order order);
}