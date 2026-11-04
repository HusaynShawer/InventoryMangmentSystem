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

        var variant = await _variants.GetByIdAsync(request.variantId);
        if (variant is null)
            throw new NotFoundException("Product variant not found.");

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

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.variantId);
        if (existingItem is not null)
        {
            existingItem.Quantity += request.quantity;
            existingItem.UnitPriceSnapshot = variant.PriceOverride ?? variant.Product.UnitPrice;
            stockLevel.ReservedQuantity -= request.quantity;

        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductVariantId = request.variantId,
                Quantity = request.quantity,
                UnitPriceSnapshot = variant.PriceOverride ?? variant.Product.UnitPrice
            });
            stockLevel.ReservedQuantity -= request.quantity;
        }
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    public async Task Delete(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        _repo.Delete(cart);
        await _unitOfWork.SaveAsync();
    }

    public async Task ClearCart(Guid userId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    public async Task RemoveItem(Guid userId, Guid itemId)
    {
        var cart = await _repo.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Cart item not found.");

        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
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
        ProductVariantId = item.ProductVariantId,
        Quantity = item.Quantity,
        UnitPriceSnapshot = item.UnitPriceSnapshot
    };


}
