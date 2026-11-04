using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly CartService _cartService;

    public CartController(ICartRepository cartRepository, CartService cartService)
    {
        _cartRepository = cartRepository;
        _cartService = cartService;
    }
    [HttpGet("")]
    public async Task<IActionResult> GetCartByUserId()
    {
        var ClaimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(ClaimUserId, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }
        var cart = await _cartService.GetByUserId(userId);
        if (cart is null)
            return NotFound();

        return Ok(cart);
    }
    [HttpGet("items")]
    public async Task<IActionResult> GetCartItems()
    {
        var ClaimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(ClaimUserId, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }
        var items = await _cartService.GetCartItems(userId);
        return Ok(items);
    }
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
    {

        var ClaimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(ClaimUserId, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }   
        await _cartService.AddToCart(userId, request.variantId, request.quantity);
        return Ok();
    }
}