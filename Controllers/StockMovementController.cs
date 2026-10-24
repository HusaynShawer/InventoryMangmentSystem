using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class StockMovementController : ControllerBase
{

    private readonly StockMovementService _service;


    public StockMovementController(StockMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovement>>> GetAllAsync()
    {
        var _stocksMovement = await _service.GetAllAsync();

        return Ok(_stocksMovement);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetByIdAsync(Guid id)
    {
        var _stockMovement = await _service.GetByIdAsync(id);
        return Ok(_stockMovement);

    }

    [HttpPost]
    public async Task<ActionResult<Stock>> Create(StockMovement stockMovement)
    {
        var _stockMovement = await _service.Create(stockMovement);

        return CreatedAtAction(
            nameof(GetByIdAsync),
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