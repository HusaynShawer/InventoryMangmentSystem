using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
public class OrderItemRepository : IOrderItemRepository
{

    private readonly ApplicationDbContext _context;
    public OrderItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<OrderItem> AddAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
        return orderItem;
    }

    public void Delete(OrderItem orderItem)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(OrderItem orderItem)
    {
        _context.Remove(orderItem);
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        var orders = await _context.OrderItems.ToListAsync();
        return orders;
    }

    public async Task<OrderItem?> GetByIdAsync(Guid id)
    {
        return await _context.OrderItems.FindAsync(id);
    }

    public void Update(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
    }
}