using System.Security.Claims;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartByUserId()
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        var cart = await _cartService.GetByUserId(userId);
        if (cart is null)
            return Ok(new CartResponseDto { UserId = userId });

        return Ok(cart);
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetCartItems()
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        var items = await _cartService.GetCartItems(userId);
        return Ok(items);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        await _cartService.AddToCart(userId, request);
        return NoContent();
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateCartItemRequestDto request)
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        await _cartService.UpdateItem(userId, itemId, request.Quantity);
        return NoContent();
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId)
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        await _cartService.RemoveItem(userId, itemId);
        return NoContent();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        await _cartService.ClearCart(userId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCart()
    {
        if (!TryGetUserId(out var userId))
            return BadRequest("Invalid user ID.");

        await _cartService.Delete(userId);
        return NoContent();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
