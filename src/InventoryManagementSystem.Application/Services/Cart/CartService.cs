using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Domain.Exceptions;

namespace InventoryManagementSystem.Application.Services;

public class CartService
{
    private readonly ICartRepository _repo;
    private readonly IProductVariantRepository _variants;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(
        ICartRepository repo,
        IProductVariantRepository variants,
        IStockLevelRepository stockLevelRepository,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _variants = variants;
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartResponseDto?> GetByUserId(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId);
        return cart is null ? null : MapCartToDto(cart);
    }

    public async Task<IEnumerable<CartItemResponseDto>> GetCartItems(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId);
        if (cart is null)
            return Enumerable.Empty<CartItemResponseDto>();

        return cart.Items.Select(MapItemToDto).ToList();
    }

    public async Task AddToCart(Guid userId, AddToCartRequestDto request)
    {
        if (request.quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var variant = await _variants.GetByIdAsync(request.variantId)
            ?? throw new NotFoundException("Product variant not found.");

        var stockLevel = await _stockLevelRepository.GetAsync(request.variantId, request.warehouseId);

        var cart = await _repo.GetByUserIdAsync(userId);
        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                Status = CartStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i =>
            i.ProductVariantId == request.variantId &&
            i.WarehouseId == request.warehouseId);
        var requestedTotal = (existingItem?.Quantity ?? 0) + request.quantity;

        if (requestedTotal > stockLevel.AvailableQuantity)
            throw new ConflictException("Insufficient available stock.");

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedTotal;
            existingItem.UnitPriceSnapshot = variant.PriceOverride ?? variant.Product?.UnitPrice ?? 0;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductVariantId = request.variantId,
                WarehouseId = request.warehouseId,
                Quantity = request.quantity,
                UnitPriceSnapshot = variant.PriceOverride ?? variant.Product?.UnitPrice ?? 0
            });
        }

        stockLevel.Reserve(request.quantity);
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateItem(Guid userId, Guid itemId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Cart item not found.");

        var stockLevel = await _stockLevelRepository.GetAsync(item.ProductVariantId, item.WarehouseId);

        var delta = quantity - item.Quantity;
        if (delta > 0)
            stockLevel.Reserve(delta);
        else if (delta < 0)
            stockLevel.Release(-delta);

        item.Quantity = quantity;
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    public async Task Delete(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        await ReleaseReservations(cart);
        _repo.Delete(cart);
        await _unitOfWork.SaveAsync();
    }

    public async Task ClearCart(Guid userId)
    {
        await ClearCartInternal(userId);
        await _unitOfWork.SaveAsync();
    }

    internal async Task ClearCartInternal(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");
        await ReleaseReservations(cart);
        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
    }
    public async Task RemoveItem(Guid userId, Guid itemId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Cart item not found.");

        var stockLevel = await _stockLevelRepository.GetAsync(item.ProductVariantId, item.WarehouseId);
        stockLevel.Release(item.Quantity);

        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    private async Task ReleaseReservations(Cart cart)
    {
        foreach (var item in cart.Items)
        {
            var stockLevel = await _stockLevelRepository.GetAsync(item.ProductVariantId, item.WarehouseId);
            stockLevel.Release(item.Quantity);
        }
    }

    private static CartResponseDto MapCartToDto(Cart cart) => new()
    {
        Id = cart.Id,
        UserId = cart.UserId,
        CreatedAt = cart.CreatedAt,
        UpdatedAt = cart.UpdatedAt,
        Items = cart.Items.Select(MapItemToDto).ToList()
    };

    private static CartItemResponseDto MapItemToDto(CartItem item) => new()
    {
        Id = item.Id,
        CartId = item.CartId,
        ProductVariantId = item.ProductVariantId,
        WarehouseId = item.WarehouseId,
        Quantity = item.Quantity,
        UnitPriceSnapshot = item.UnitPriceSnapshot
    };
}
