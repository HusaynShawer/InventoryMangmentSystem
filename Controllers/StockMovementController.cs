using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class StockMovementController : ControllerBase
{

    private readonly GStockMovementService _service;


    public StockMovementController(GStockMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovement>>> GetAll()
    {
        var _stocksMovement = await _service.GetAll();

        return Ok(_stocksMovement);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetById(Guid id)
    {
        var _stockMovement = await _service.GetByID(id);
        return Ok(_stockMovement);

    }

    [HttpPost]
    public async Task<ActionResult<Stock>> Create(StockMovement stockMovement)
    {
        var _stockMovement = await _service.Create(stockMovement);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _stockMovement.Id},
            _stockMovement
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, StockMovement stockMovement)
    {
        if (id != stockMovement.Id)
            throw new BadRequestException(
                "Route id does not match the stock movement id.");

        await _service.Update(stockMovement);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);

        return NoContent();
    }
}