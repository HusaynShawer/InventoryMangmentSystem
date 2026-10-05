using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public async Task<ActionResult<Stock>> GetById(int id)
    {
        var stock = await _service.GetByID(id);
        return stock is null ? NotFound("Stock not found") : Ok(stock);
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
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, Stock stock)
    {
        if (id != stock.Id)
            return BadRequest("ID mismatch.");

        await _service.Update(stock);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}