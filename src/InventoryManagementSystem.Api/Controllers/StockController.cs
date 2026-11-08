using System.Security.Claims;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class StockController : ControllerBase
{
    private readonly StockService _service;

    public StockController(StockService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockResponseDto>>> GetAllAsync()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                    ?? User.FindFirst("nameid")?.Value;
        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var userId))
            return Unauthorized();
        var stocks = await _service.GetAllAsync(userId);
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockLevel>> GetByIdAsync(Guid id)
    {
        var stock = await _service.GetByIdAsync(id);
        return Ok(stock);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<StockLevel>> Create(StockDto dto)
    {
        var created = await _service.Create(dto);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = created.Id },
            created
        );
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, StockDto dto)
    {
        await _service.Update(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    [HttpPut("adjust")]
    public async Task<IActionResult> Adjust(
        [FromBody] StockAdjustmentDto stockAdjustmentDTO)
    {
        var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(claimUserId, out var userId))
            return Unauthorized();
        await _service.AdjustStock(userId, stockAdjustmentDTO);
        return NoContent();
    }

    [HttpGet("lowstock")]
    public async Task<ActionResult<IEnumerable<LowStockDto>>> lowStock(Guid warehouseId)
    {
        var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(claimUserId, out var userId))
            return Unauthorized();
        
        var _lowStocks = await _service.lowStockDto(warehouseId, userId);
        return Ok(_lowStocks);
    }
    [HttpPost("Transfer")]
    public async Task<ActionResult> Transfer([FromBody] TransferDto dto)
    {
        var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(claimUserId, out var userId))
            return Unauthorized();
        
        await _service.Transfer(userId, dto);
        return NoContent();
        
    }

}