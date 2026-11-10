using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Abstractions.Persistence;

namespace InventoryManagementSystem.Application.Services;
public class OrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;

    public OrderItemService(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto orderItemDto)
    {
        var orderItem = new OrderItem
        {
            OrderId = orderItemDto.OrderId,
            WarehouseId = orderItemDto.WarehouseId,
            ProductVariantId = orderItemDto.ProductVariantId,
            Quantity = orderItemDto.Quantity,
            UnitPrice = orderItemDto.UnitPrice
        };

        var createdOrderItem = await _orderItemRepository.AddAsync(orderItem);
        return new OrderItemDto
        {
            Id = createdOrderItem.Id,
            WarehouseId = createdOrderItem.WarehouseId,
            ProductVariantId = createdOrderItem.ProductVariantId,
            Quantity = createdOrderItem.Quantity,
            UnitPrice = createdOrderItem.UnitPrice
        };
    }
}