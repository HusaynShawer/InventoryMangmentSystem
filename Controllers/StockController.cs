using System.Security.Claims;
using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class StockController : ControllerBase
{
    private readonly GStockService _service;

    public StockController(GStockService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
    {
        var stocks = await _service.GetAll();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Stock>> GetById(Guid id)
    {
        var stock = await _service.GetByID(id);
        return Ok(stock);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Stock>> Create(Stock stock)
    {
        var created = await _service.Create(stock);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, Stock stock)
    {
        if (id != stock.Id)
            throw new BadRequestException(
                "Route id does not match the stock id.");

        await _service.Update(stock);
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
        [FromBody] StockAdjustmentDTO stockAdjustmentDTO)
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