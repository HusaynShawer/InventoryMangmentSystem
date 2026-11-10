using InventoryManagementSystem.Domain.Enums; 
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Abstractions.Persistence;
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly CartService _cartService;
    private readonly IUnitOfWork _unitOfWork;
    public OrderService(IOrderRepository orderRepository,
                        CartService cartService,
                        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _cartService = cartService;
        _unitOfWork = unitOfWork;
    }

    // Implement methods for order management (e.g., CreateOrder, UpdateOrderStatus, etc.)
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
            // Check if the user has an existing cart
            var cart = await _cartService.GetByUserId(createOrderDto.UserId)
                ?? throw new Exception("Cart not found for the user.");
            if (!cart.Items.Any())
                throw new Exception("Cart is empty. Cannot create an order from an empty cart.");

            // Create a new order based on the cart items and the provided order details
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = createOrderDto.UserId,
                ShippingAddressSnapshot = createOrderDto.ShippingAddressSnapshot,
                IdempotencyKey = createOrderDto.IdempotencyKey,
                Currency = createOrderDto.Currency,
                ShippingAmount = createOrderDto.ShippingAmount,
                DiscountAmount = createOrderDto.DiscountAmount,
                Subtotal = cart.Items.Sum(item => item.UnitPriceSnapshot * item.Quantity),
                TaxAmount = 0, // Calculate tax if applicable
                TotalAmount = cart.Items.Sum(item => item.UnitPriceSnapshot * item.Quantity) + createOrderDto.ShippingAmount - createOrderDto.DiscountAmount,
                Items = cart.Items.Select(item => new OrderItem
                {
                    ProductVariantId = item.ProductVariantId,
                    WarehouseId = item.WarehouseId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPriceSnapshot
                }).ToList()
            };

            var createdOrder = await _orderRepository.AddAsync(order);
            // clear the cart after creating the order
            await _cartService.ClearCartInternal(createOrderDto.UserId);
            await _unitOfWork.SaveAsync();
            return new OrderDto
            {
                Id = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                UserId = createdOrder.UserId,
                Status = createdOrder.Status,
                Subtotal = createdOrder.Subtotal,
                ShippingAmount = createdOrder.ShippingAmount,
                DiscountAmount = createdOrder.DiscountAmount,
                TaxAmount = createdOrder.TaxAmount,
                TotalAmount = createdOrder.TotalAmount,
                Currency = createdOrder.Currency,
                ShippingAddressSnapshot = createdOrder.ShippingAddressSnapshot,
                ReservedAt = createdOrder.ReservedAt,
                ReservationExpiresAt = createdOrder.ReservationExpiresAt,
                CreatedAt = createdOrder.CreatedAt,
                UpdatedAt = createdOrder.UpdatedAt,
                Items = createdOrder.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,
                    WarehouseId = item.WarehouseId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    Status = item.Status
                }).ToList()
            };
        });
    }
    private static string GenerateOrderNumber()
    {
        var now = DateTime.UtcNow;
        var sequence = (now.Ticks / 10) % 1_000_000_000;
        return $"ORD-{now:yyyy}-{sequence:D9}";
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto updateOrderStatusDto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new Exception("Order not found.");

        order.Status = updateOrderStatusDto.Status;
        await _unitOfWork.SaveAsync();
    }
}